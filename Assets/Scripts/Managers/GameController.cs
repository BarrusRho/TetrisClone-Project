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

        private float _dropInterval = 1f;
        private float _timeToDrop;

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
            if (!_spawner) return;
            
            if (_activeShape == null)
            {
                _activeShape = _spawner.SpawnShape();
            }

            _spawner.transform.position = Vectorf.Round(_spawner.transform.position);
        }

        private void Update()
        {
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
                }
            }

        }
    }
}