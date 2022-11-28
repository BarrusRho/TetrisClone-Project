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

        private float _dropInterval = 0.25f;
        private float _timeToDrop;
        private float _timeToNextKey;
        
        [SerializeField] [Range(0.02f, 1)]
        private float _keyRepeatRate = 0.25f;

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
            _timeToNextKey = Time.time;
            
            if (!_spawner) return;
            
            if (_activeShape == null)
            {
                _activeShape = _spawner.SpawnShape();
            }

            _spawner.transform.position = Vectorf.Round(_spawner.transform.position);
        }

        private void Update()
        {
            if (Input.GetButton("MoveRight") && Time.time > _timeToNextKey || Input.GetButtonDown("MoveRight"))
            {
                _activeShape.MoveRight();
                _timeToNextKey = Time.time + _keyRepeatRate;

                if (_gameBoard.IsValidPosition(_activeShape))
                {
                    Debug.Log($"Move right");
                }
                else
                {
                    _activeShape.MoveLeft();
                    Debug.Log($"Hit the rightmost boundary");
                }
            }
            
            if (!_gameBoard || !_spawner)
            {
                return;
            }

            if (Time.time > _timeToDrop)
            {
                _timeToDrop = Time.time + _dropInterval;
                
                if (_activeShape)
                {
                    _activeShape.MoveDown();

                    if (!_gameBoard.IsValidPosition(_activeShape))
                    {
                        _activeShape.MoveUp();
                        _gameBoard.StoreShapeInGrid(_activeShape);

                        if (_spawner)
                        {
                            _activeShape = _spawner.SpawnShape();
                        }
                    }
                }
            }

        }
    }
}