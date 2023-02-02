using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TetrisClone.Core
{
    public class Spawner : MonoBehaviour
    {
        private float _queueScale = 0.5f;
        private Shape[] _queuedShapes = new Shape[3];
        
        public Shape[] allShapes;
        public Transform[] queuedTransforms = new Transform[3];

        private void Awake()
        {
            InitialiseQueue();
        }
        
        private Shape GetRandomShape()
        {
            var i = Random.Range(0, allShapes.Length);
            if (allShapes[i])
            {
                return allShapes[i];
            }
            else
            {
                Debug.Log($"WARNING! Invalid shape in Spawner");
                return null;
            }
        }

        public Shape SpawnShape()
        {
            Shape shape = null;
            //shape = Instantiate(GetRandomShape(), transform.position, Quaternion.identity) as Shape;
            shape = GetQueuedShape();
            shape.transform.position = this.transform.position;
            shape.transform.localScale = Vector3.one;
            
            if (shape)
            {
                return shape;
            }
            else
            {
                Debug.Log($"WARNING! Invalid shape in Spawner");
                return null;
            }
        }

        private void InitialiseQueue()
        {
            for (int i = 0; i < _queuedShapes.Length; i++)
            {
                _queuedShapes[i] = null;
            }
            
            FillQueue();
        }

        private void FillQueue()
        {
            for (int i = 0; i < _queuedShapes.Length; i++)
            {
                if (!_queuedShapes[i])
                {
                    _queuedShapes[i] = Instantiate(GetRandomShape(), transform.position, Quaternion.identity) as Shape;
                    _queuedShapes[i].transform.position = queuedTransforms[i].position + _queuedShapes[i].queueOffset;
                    _queuedShapes[i].transform.localScale = new Vector3(_queueScale, _queueScale, _queueScale);
                }
            }
        }

        private Shape GetQueuedShape()
        {
            Shape firstShape = null;

            if (_queuedShapes[0])
            {
                firstShape = _queuedShapes[0];
            }

            for (int i = 1; i < _queuedShapes.Length; i++)
            {
                _queuedShapes[i - 1] = _queuedShapes[i];
                _queuedShapes[i - 1].transform.position = queuedTransforms[i - 1].position + _queuedShapes[i].queueOffset;
            }

            _queuedShapes[_queuedShapes.Length - 1] = null;
            
            FillQueue();

            return firstShape;
        }
    }
}