using System;
using TetrisClone.Utility;
using UnityEngine;

namespace TetrisClone.Core
{
    public class Shape : MonoBehaviour
    {
        public bool canRotate = true;
        public Vector3 queueOffset;
        public string glowingSquareTag = "LandShapeFX";
        
        private GameObject[] _glowingSquareFX;

        private void Start()
        {
            if (glowingSquareTag != "")
            {
                _glowingSquareFX = GameObject.FindGameObjectsWithTag(glowingSquareTag);
            }
        }

        private void Move(Vector3 moveDirection)
        {
            transform.position += moveDirection;
        }

        public void MoveLeft()
        {
            Move(new Vector3(-1, 0, 0));
        }

        public void MoveRight()
        {
            Move(new Vector3(1, 0, 0));
        }

        public void MoveDown()
        {
            Move(new Vector3(0, -1, 0));
        }

        public void MoveUp()
        {
            Move(new Vector3(0, 1, 0));
        }

        public void RotateRight()
        {
            if (canRotate)
            {
                transform.Rotate(0, 0, -90);
            }
        }

        public void RotateLeft()
        {
            if (canRotate)
            {
                transform.Rotate(0, 0, 90);
            }
        }

        public void RotateClockwise(bool clockwise)
        {
            if (clockwise)
            {
                RotateRight();
            }
            else
            {
                RotateLeft();
            }
        }

        public void LandShapeFX()
        {
            var index = 0;

            foreach (Transform child in gameObject.transform)
            {
                if (_glowingSquareFX[index])
                {
                    _glowingSquareFX[index].transform.position = new Vector3(child.position.x, child.position.y, -2f);
                    var particleUtility = _glowingSquareFX[index].GetComponent<ParticleUtility>();

                    if (particleUtility)
                    {
                        particleUtility.PlayParticles();
                    }
                }

                index++;
            }
        }
    }
}