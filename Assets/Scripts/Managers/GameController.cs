using System;
using UnityEngine;
using TetrisClone.Core;
using TetrisClone.Utility;

namespace TetrisClone.Managers
{
    public class GameController : MonoBehaviour
    {
        private AudioManager _audioManager;
        
        private Board _gameBoard;
        private Spawner _spawner;
        private Shape _activeShape;

        public float _dropInterval = 0.9f;
        private float _timeToDrop;
        private bool _isGameOver = false;

        public GameObject gameOverPanel;
        //private float _timeToNextKey;

        //[SerializeField] [Range(0.02f, 1)] private float _keyRepeatRate = 0.25f;
        
        private float _timeToNextKeyLeftRight;
        [SerializeField] [Range(0.02f, 1f)] private float _keyRepeatRateLeftRight = 0.15f;

        private float _timeToNextKeyRotate;
        [SerializeField] [Range(0.02f, 1f)] private float _keyRepeateRateRotate = 0.25f;
        
        private float _timeToNextKeyDown;
        [SerializeField] [Range(0.01f, 1f)] private float _keyRepeatRateDown = 0.01f;

        private void Awake()
        {
            _audioManager = FindObjectOfType<AudioManager>();
            
            _gameBoard = GameObject.FindWithTag("Board").GetComponent<Board>();
            _spawner = GameObject.FindWithTag("Spawner").GetComponent<Spawner>();

            if (!_gameBoard || !_spawner)
            {
                Debug.Log($"WARNING. There is a missing assignment");
            }
        }

        private void Start()
        {
            //_timeToNextKey = Time.time;
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
        }

        private void Update()
        {
            if (!_gameBoard || !_spawner || !_activeShape || _isGameOver || !_audioManager)
            {
                return;
            }

            PlayerInput();
        }

        private void PlayerInput()
        {
            if (Input.GetButton("MoveRight") && Time.time > _timeToNextKeyLeftRight || Input.GetButtonDown("MoveRight") ||
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
                    PlaySound(_audioManager.moveSound,1f );
                }
            }
            else if (Input.GetButton("MoveLeft") && Time.time > _timeToNextKeyLeftRight || Input.GetButtonDown("MoveLeft") ||
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
            else if (Input.GetButtonDown("Rotate") && Time.time > _timeToNextKeyRotate || Input.GetKeyDown(KeyCode.UpArrow))
            {
                _activeShape.RotateRight();
                _timeToNextKeyRotate = Time.time + _keyRepeateRateRotate;

                if (!_gameBoard.IsValidPosition(_activeShape))
                {
                    _activeShape.RotateLeft();
                    PlaySound(_audioManager.errorSound, 0.5f);
                }
                else
                {
                    PlaySound(_audioManager.moveSound, 1f);
                }
            }
            else if (Input.GetButton("MoveDown") && (Time.time > _timeToNextKeyDown) || (Time.time > _timeToDrop))
            {
                _timeToDrop = Time.time + _dropInterval;
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
        }


        private void LandShape()
        {
            _activeShape.MoveUp();
            _gameBoard.StoreShapeInGrid(_activeShape);
            PlaySound(_audioManager.dropSound, 1f);
            _activeShape = _spawner.SpawnShape();
            
            _timeToNextKeyLeftRight = Time.time;
            _timeToNextKeyRotate = Time.time;
            _timeToNextKeyDown = Time.time;
            
            _gameBoard.ClearAllRows();

            if (_gameBoard.completedRows > 0)
            {
                PlaySound(_audioManager.clearRowSound, 1f);
            }
        }
        
        private void PlaySound(AudioClip audioClip, float volumeMultiplier)
        {
            if (_audioManager.isSFXEnabled && audioClip)
            {
                AudioSource.PlayClipAtPoint(audioClip, Camera.main.transform.position, Mathf.Clamp(_audioManager.sFXVolume * volumeMultiplier,0.05f, 1f));
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
            
            _isGameOver = true;
        }
        
        public void Restart()
        {
            Debug.Log($"Level restarted");
            Application.LoadLevel(Application.loadedLevel);
        }
    }
}
