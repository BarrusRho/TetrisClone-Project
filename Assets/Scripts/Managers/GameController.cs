using System;
using UnityEngine;
using TetrisClone.Core;
using TetrisClone.Utility;

namespace TetrisClone.Managers
{
    public class GameController : MonoBehaviour
    {
        private Board _gameBoard;
        private Spawner _spawner;
        private Shape _activeShape;

        private float _dropInterval = 0.9f;
        private float _timeToDrop;
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
        }

        private void Update()
        {
            if (!_gameBoard || !_spawner || !_activeShape)
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
                }
            }
            else if (Input.GetButtonDown("Rotate") && Time.time > _timeToNextKeyRotate || Input.GetKeyDown(KeyCode.UpArrow))
            {
                _activeShape.RotateRight();
                _timeToNextKeyRotate = Time.time + _keyRepeateRateRotate;

                if (!_gameBoard.IsValidPosition(_activeShape))
                {
                    _activeShape.RotateLeft();
                }
            }
            else if (Input.GetButton("MoveDown") && (Time.time > _timeToNextKeyDown) || (Time.time > _timeToDrop))
            {
                _timeToDrop = Time.time + _dropInterval;
                _timeToNextKeyDown = Time.time + _keyRepeatRateDown;

                _activeShape.MoveDown();

                if (!_gameBoard.IsValidPosition(_activeShape))
                {
                    LandShape();
                }
            }
        }

        private void LandShape()
        {
            //_timeToNextKey = Time.time;
            _timeToNextKeyLeftRight = Time.time;
            _timeToNextKeyRotate = Time.time;
            _timeToNextKeyDown = Time.time;

            _activeShape.MoveUp();
            _gameBoard.StoreShapeInGrid(_activeShape);
            _activeShape = _spawner.SpawnShape();
        }
    }
}