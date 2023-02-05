using System;
using System.Collections;
using TetrisClone.Utility;
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
        public ParticleUtility spawnFX;

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
            StartCoroutine(GrowShapeRoutine(shape, transform.position, 0.25f));
            shape.transform.localScale = Vector3.one;

            if (spawnFX)
            {
                spawnFX.PlayParticles();
            }
            
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

        private IEnumerator GrowShapeRoutine(Shape shape, Vector3 position, float growTime = 0.5f)
        {
            var startSize = 0f;
            growTime = Mathf.Clamp(growTime, 0.1f, 2f);
            var sizeDelta = Time.deltaTime / growTime;

            while (startSize < 1f)
            {
                shape.transform.localScale = new Vector3(startSize, startSize, startSize);
                startSize += sizeDelta;
                shape.transform.position = position;
                yield return null;
            }
            
            shape.transform.localScale = Vector3.one;
        }
    }
}