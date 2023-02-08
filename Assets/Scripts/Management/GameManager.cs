using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TetrisClone.Core;
using TetrisClone.Utility;
using TMPro;
using UnityEngine.Serialization;

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
        private float _timeToNextDrag;
        private float _timeToNextSwipe;
        [Range(0.05f, 1f)] public float minimumTimeToDrag = 0.15f;
        [SerializeField] [Range(0.05f, 1f)] private float _minimumTimeToSwipe = 0.3f;
        private bool _hasTapped = false;

        private enum Direction
        {
            none,
            left,
            right,
            up,
            down
        }

        private Direction _dragDirection = Direction.none;
        private Direction _swipeDirection = Direction.none;

        public float dropInterval = 0.9f;
        private float _dropIntervalModded;
        private float _timeToDrop;
        private bool _isGameOver = false;
        public bool isPaused = false;
        public GameObject gameOverPanel;
        public IconToggle rotationIconToggle;
        public GameObject pauseMenuPanel;

        public ParticleUtility gameOverFX;
        public TMP_Text diagnosticText;

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

        private void OnEnable()
        {
            TouchManager.DragEvent += DragHandler;
            TouchManager.SwipeEvent += SwipeHandler;
            TouchManager.TapEvent += TapHandler;
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

            if (diagnosticText)
            {
                diagnosticText.text = "";
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

        private void OnDisable()
        {
            TouchManager.DragEvent -= DragHandler;
            TouchManager.SwipeEvent -= SwipeHandler;
            TouchManager.TapEvent -= TapHandler;
        }

        private void PlayerInput()
        {
            #region KeyboardControls

            if ((Input.GetButton("MoveRight") && (Time.time > _timeToNextKeyLeftRight)) ||
                Input.GetButtonDown("MoveRight") ||
                Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveRight();
            }
            else if ((Input.GetButton("MoveLeft") && (Time.time > _timeToNextKeyLeftRight)) ||
                     Input.GetButtonDown("MoveLeft") ||
                     Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveLeft();
            }
            else if (Input.GetButtonDown("Rotate") && Time.time > _timeToNextKeyRotate ||
                     Input.GetKeyDown(KeyCode.UpArrow))
            {
                Rotate();
            }
            else if ((Input.GetButton("MoveDown") && (Time.time > _timeToNextKeyDown)) || (Time.time > _timeToDrop))
            {
                MoveDown();
            }

            #endregion

            #region TouchControls

            else if ((_swipeDirection == Direction.right && Time.time > _timeToNextSwipe) ||
                     (_dragDirection == Direction.right && Time.time > _timeToNextDrag))
            {
                MoveRight();
                _timeToNextDrag = Time.time + minimumTimeToDrag;
                _timeToNextSwipe = Time.time + _minimumTimeToSwipe;
            }
            else if ((_swipeDirection == Direction.left && Time.time > _timeToNextSwipe) ||
                     (_dragDirection == Direction.left && Time.time > _timeToNextDrag))
            {
                MoveLeft();
                _timeToNextDrag = Time.time + minimumTimeToDrag;
                _timeToNextSwipe = Time.time + _minimumTimeToSwipe;
            }
            else if ((_swipeDirection == Direction.up && Time.time > _timeToNextSwipe) || (_hasTapped))
            {
                Rotate();
                _timeToNextSwipe = Time.time + _minimumTimeToSwipe;
                _hasTapped = false;
            }
            else if (_dragDirection == Direction.down && Time.time > _timeToNextDrag)
            {
                MoveDown();
                //_timeToNextDrag = Time.time + _minimumTimeToDrag;
            }

            #endregion

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

            _dragDirection = Direction.none;
            _swipeDirection = Direction.none;
            _hasTapped = false;
        }

        private void MoveDown()
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

        private void Rotate()
        {
            _activeShape.RotateClockwise(_clockwiseRotation);
            _timeToNextKeyRotate = Time.time + _keyRepeateRateRotate;

            if (!_gameBoard.IsValidPosition(_activeShape))
            {
                _activeShape.RotateClockwise(!_clockwiseRotation);
                _audioManager.PlaySound(_audioManager.errorSound, 0.5f);
            }
            else
            {
                _audioManager.PlaySound(_audioManager.moveSound, 1f);
            }
        }

        private void MoveLeft()
        {
            _activeShape.MoveLeft();
            _timeToNextKeyLeftRight = Time.time + _keyRepeatRateLeftRight;

            if (!_gameBoard.IsValidPosition(_activeShape))
            {
                _activeShape.MoveRight();
                _audioManager.PlaySound(_audioManager.errorSound, 0.5f);
            }
            else
            {
                _audioManager.PlaySound(_audioManager.moveSound, 1f);
            }
        }

        private void MoveRight()
        {
            _activeShape.MoveRight();
            _timeToNextKeyLeftRight = Time.time + _keyRepeatRateLeftRight;

            if (!_gameBoard.IsValidPosition(_activeShape))
            {
                _activeShape.MoveLeft();
                _audioManager.PlaySound(_audioManager.errorSound, 0.5f);
            }
            else
            {
                _audioManager.PlaySound(_audioManager.moveSound, 1f);
            }
        }

        private void LandShape()
        {
            if (!_activeShape)
            {
                return;
            }

            _activeShape.MoveUp();
            _gameBoard.StoreShapeInGrid(_activeShape);
            _activeShape.LandShapeFX();
            _audioManager.PlaySound(_audioManager.dropSound, 1f);

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

            _gameBoard.StartCoroutine(nameof(_gameBoard.ClearAllRows));

            if (_gameBoard.completedRows > 0)
            {
                _scoreManager.ScoreLines(_gameBoard.completedRows);

                if (_scoreManager.hasLeveledUp)
                {
                    _audioManager.PlaySound(_audioManager.levelUpVocalClip, 1f);
                    _dropIntervalModded = Mathf.Clamp(dropInterval - (((float)_scoreManager.level - 1) * 0.1f),
                        0.05f, 1f);
                }
                else
                {
                    if (_gameBoard.completedRows > 1)
                    {
                        var randomVocalAudioClip = _audioManager.GetRandomAudioClip(_audioManager.vocalAudioClips);
                        _audioManager.PlaySound(randomVocalAudioClip, 1f);
                    }
                }

                _audioManager.PlaySound(_audioManager.clearRowSound, 1f);
            }
        }
        
        private void GameOver()
        {
            _activeShape.MoveUp();

            StartCoroutine(GameOverRoutine());

            _audioManager.PlaySound(_audioManager.gameOverSound, 2f);
            _audioManager.PlaySound(_audioManager.gameOverVocal, 2f);

            _isGameOver = true;
        }

        public void Restart()
        {
            Debug.Log($"Level restarted");
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
                _audioManager.PlaySound(_audioManager.holdSound, 1f);
            }
            else if (_shapeHolder.canReleaseShape)
            {
                var temporaryShape = _activeShape;
                _activeShape = _shapeHolder.ReleaseShape();
                _activeShape.transform.position = _spawner.transform.position;
                _shapeHolder.CatchActiveShape(temporaryShape);
                _audioManager.PlaySound(_audioManager.holdSound, 1f);
            }
            else
            {
                Debug.LogWarning("ShapeHolder Warning: Wait for cooldown");
                _audioManager.PlaySound(_audioManager.errorSound, 1f);
            }

            if (_ghostShapeManager)
            {
                _ghostShapeManager.ResetGhostShape();
            }
        }

        private IEnumerator GameOverRoutine()
        {
            if (gameOverFX)
            {
                gameOverFX.PlayParticles();
            }

            yield return new WaitForSeconds(0.3f);

            if (gameOverPanel)
            {
                gameOverPanel.SetActive(true);
            }
        }

        private void DragHandler(Vector2 dragMovement)
        {
            if (diagnosticText)
            {
                diagnosticText.text = $"SwipeEvent detected";
            }

            _dragDirection = GetTouchDirection(dragMovement);
        }

        private void SwipeHandler(Vector2 swipeMovement)
        {
            if (diagnosticText)
            {
                diagnosticText.text = "";
            }

            _swipeDirection = GetTouchDirection(swipeMovement);
        }

        private void TapHandler(Vector2 tapMovement)
        {
            if (diagnosticText)
            {
                diagnosticText.text = "";
            }

            _hasTapped = true;

            //_swipeDirection = GetTouchDirection(tapMovement);
        }

        private Direction GetTouchDirection(Vector2 swipeMovement)
        {
            var swipeDirection = Direction.none;

            if (Mathf.Abs(swipeMovement.x) > Mathf.Abs(swipeMovement.y))
            {
                swipeDirection = (swipeMovement.x >= 0) ? Direction.right : Direction.left;
            }
            else
            {
                swipeDirection = (swipeMovement.y >= 0) ? Direction.up : Direction.down;
            }

            return swipeDirection;
        }
    }
}