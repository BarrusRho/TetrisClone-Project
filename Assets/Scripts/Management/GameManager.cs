using System;
using UnityEngine;
using TetrisClone.Core;
using TetrisClone.Utility;

namespace TetrisClone.Management
{
    public class GameManager : MonoBehaviour
    {
        private AudioManager _audioManager;
        private ScoreManager _scoreManager;
        private GhostShapeManager _ghostShapeManager;
        private ShapeHolder _shapeHolder;

        private Board _gameBoard;
        private Spawner _spawner;
        private Shape _activeShape;
        private bool _clockwiseRotation = true;

        private float _timeToNextKeyLeftRight;
        [SerializeField] [Range(0.02f, 1f)] private float _keyRepeatRateLeftRight = 0.15f;
        private float _timeToNextKeyRotate;
        [SerializeField] [Range(0.02f, 1f)] private float _keyRepeateRateRotate = 0.25f;
        private float _timeToNextKeyDown;
        [SerializeField] [Range(0.01f, 1f)] private float _keyRepeatRateDown = 0.01f;

        public float dropInterval = 0.9f;
        private float _dropIntervalModded;
        private float _timeToDrop;
        private bool _isGameOver = false;
        public bool isPaused = false;
        public GameObject gameOverPanel;
        public IconToggle rotationIconToggle;
        public GameObject pauseMenuPanel;


        private void Awake()
        {
            _audioManager = FindObjectOfType<AudioManager>();
            _scoreManager = FindObjectOfType<ScoreManager>();
            _ghostShapeManager = FindObjectOfType<GhostShapeManager>();
            _shapeHolder = FindObjectOfType<ShapeHolder>();

            _gameBoard = GameObject.FindWithTag("Board").GetComponent<Board>();
            _spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();

            if (!_gameBoard || !_spawner || !_audioManager || !_scoreManager)
            {
                Debug.Log($"WARNING. There is a missing assignment");
            }
        }

        private void Start()
        {
            _timeToNextKeyLeftRight = Time.time + _keyRepeatRateLeftRight;
            _timeToNextKeyRotate = Time.time + _keyRepeateRateRotate;
            _timeToNextKeyDown = Time.time + _keyRepeatRateDown;

            if (!_spawner) return;

            if (_activeShape == null)
            {
                _activeShape = _spawner.SpawnShape();
            }

            _spawner.transform.position = Vectorf.Round(_spawner.transform.position);

            if (gameOverPanel)
            {
                gameOverPanel.SetActive(false);
            }
            if (pauseMenuPanel)
            {
                pauseMenuPanel.SetActive(false);
            }

            _dropIntervalModded = dropInterval;
        }

        private void Update()
        {
            if (!_gameBoard || !_spawner || !_activeShape || _isGameOver || !_audioManager || !_scoreManager)
            {
                return;
            }

            PlayerInput();
        }

        private void LateUpdate()
        {
            if (_ghostShapeManager)
            {
                _ghostShapeManager.DrawGhostShape(_activeShape, _gameBoard);
            }
        }

        private void PlayerInput()
        {
            if ((Input.GetButton("MoveRight") && (Time.time > _timeToNextKeyLeftRight)) ||
                Input.GetButtonDown("MoveRight") ||
                Input.GetKeyDown(KeyCode.RightArrow))
            {
                _activeShape.MoveRight();
                _timeToNextKeyLeftRight = Time.time + _keyRepeatRateLeftRight;

                if (!_gameBoard.IsValidPosition(_activeShape))
                {
                    _activeShape.MoveLeft();
                    PlaySound(_audioManager.errorSound, 0.5f);
                }
                else
                {
                    PlaySound(_audioManager.moveSound, 1f);
                }
            }
            else if ((Input.GetButton("MoveLeft") && (Time.time > _timeToNextKeyLeftRight)) ||
                     Input.GetButtonDown("MoveLeft") ||
                     Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _activeShape.MoveLeft();
                _timeToNextKeyLeftRight = Time.time + _keyRepeatRateLeftRight;

                if (!_gameBoard.IsValidPosition(_activeShape))
                {
                    _activeShape.MoveRight();
                    PlaySound(_audioManager.errorSound, 0.5f);
                }
                else
                {
                    PlaySound(_audioManager.moveSound, 1f);
                }
            }
            else if (Input.GetButtonDown("Rotate") && Time.time > _timeToNextKeyRotate ||
                     Input.GetKeyDown(KeyCode.UpArrow))
            {
                //_activeShape.RotateRight();
                _activeShape.RotateClockwise(_clockwiseRotation);
                _timeToNextKeyRotate = Time.time + _keyRepeateRateRotate;

                if (!_gameBoard.IsValidPosition(_activeShape))
                {
                    _activeShape.RotateClockwise(!_clockwiseRotation);
                    PlaySound(_audioManager.errorSound, 0.5f);
                }
                else
                {
                    PlaySound(_audioManager.moveSound, 1f);
                }
            }
            else if ((Input.GetButton("MoveDown") && (Time.time > _timeToNextKeyDown)) || (Time.time > _timeToDrop))
            {
                _timeToDrop = Time.time + _dropIntervalModded;
                _timeToNextKeyDown = Time.time + _keyRepeatRateDown;

                _activeShape.MoveDown();

                if (!_gameBoard.IsValidPosition(_activeShape))
                {
                    if (_gameBoard.IsOverLimit(_activeShape))
                    {
                        GameOver();
                    }
                    else
                    {
                        LandShape();
                    }
                }
            }
            else if (Input.GetButtonDown("ToggleRotation"))
            {
                ToggleRotationDirection();
            }
            else if (Input.GetButtonDown("Pause"))
            {
                TogglePause();
            }
            else if (Input.GetButtonDown("Hold"))
            {
                HoldShape();
            }
        }


        private void LandShape()
        {
            _activeShape.MoveUp();
            _gameBoard.StoreShapeInGrid(_activeShape);
            PlaySound(_audioManager.dropSound, 1f);

            if (_ghostShapeManager)
            {
                _ghostShapeManager.ResetGhostShape();
            }

            if (_shapeHolder)
            {
                _shapeHolder.canReleaseShape = true;
            }
            
            _activeShape = _spawner.SpawnShape();

            _timeToNextKeyLeftRight = Time.time;
            _timeToNextKeyRotate = Time.time;
            _timeToNextKeyDown = Time.time;

            _gameBoard.ClearAllRows();

            if (_gameBoard.completedRows > 0)
            {
                _scoreManager.ScoreLines(_gameBoard.completedRows);

                if (_scoreManager.hasLeveledUp)
                {
                    PlaySound(_audioManager.levelUpVocalClip, 1f);
                    _dropIntervalModded = Mathf.Clamp(dropInterval - (((float)_scoreManager.level -1) * 0.1f), 0.05f, 1f);
                }
                else
                {
                    if (_gameBoard.completedRows > 1)
                    {
                        var randomVocalAudioClip = _audioManager.GetRandomAudioClip(_audioManager.vocalAudioClips);
                        PlaySound(randomVocalAudioClip, 1f);
                    }
                }

                PlaySound(_audioManager.clearRowSound, 1f);
            }
        }

        private void PlaySound(AudioClip audioClip, float volumeMultiplier)
        {
            if (_audioManager.isSFXEnabled && audioClip)
            {
                AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position,
                    Mathf.Clamp(_audioManager.sFXVolume * volumeMultiplier, 0.05f, 1f));
            }
        }

        private void GameOver()
        {
            _activeShape.MoveUp();

            if (gameOverPanel)
            {
                gameOverPanel.SetActive(true);
            }

            PlaySound(_audioManager.gameOverSound, 2f);
            PlaySound(_audioManager.gameOverVocal, 2f);

            _isGameOver = true;
        }

        public void Restart()
        {
            Debug.Log($"Level restarted");
            Time.timeScale = 1f;
            Application.LoadLevel(Application.loadedLevel);
        }

        public void ToggleRotationDirection()
        {
            _clockwiseRotation = !_clockwiseRotation;
            if (rotationIconToggle)
            {
                rotationIconToggle.ToggleIcon(_clockwiseRotation);
            }
        }

        public void TogglePause()
        {
            if (_isGameOver)
            {
                return;
            }

            isPaused = !isPaused;

            if (pauseMenuPanel)
            {
                pauseMenuPanel.SetActive(isPaused);

                if (_audioManager)
                {
                    _audioManager.audioSource.volume =
                        (isPaused) ? _audioManager.musicVolume * 0.25f : _audioManager.musicVolume;
                }

                Time.timeScale = (isPaused) ? 0 : 1;
            }
        }

        public void HoldShape()
        {
            if (!_shapeHolder)
            {
                return;
            }

            if (!_shapeHolder.holdShape)
            {
                _shapeHolder.CatchActiveShape(_activeShape);
                _activeShape = _spawner.SpawnShape();
                PlaySound(_audioManager.holdSound, 1f);
            }
            else if (_shapeHolder.canReleaseShape)
            {
                var temporaryShape = _activeShape;
                _activeShape = _shapeHolder.ReleaseShape();
                _activeShape.transform.position = _spawner.transform.position;
                _shapeHolder.CatchActiveShape(temporaryShape);
                PlaySound(_audioManager.holdSound, 1f);
            }
            else
            {
                Debug.LogWarning("ShapeHolder Warning: Wait for cooldown");
                PlaySound(_audioManager.errorSound, 1f);
            }

            if (_ghostShapeManager)
            {
                _ghostShapeManager.ResetGhostShape();
            }
        }
    }
}