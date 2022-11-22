using System;
using UnityEngine;

namespace TetrisClone.Core
{
    public class Board : MonoBehaviour
    {
        public Transform emptySprite;
        public int height = 30;
        public int width = 10;
        public int header = 8;

        private Transform[,] _grid;
        
        private void Start()
        {
            _grid = new Transform[width, height];
            
            DrawEmptyCells();
        }

        private void DrawEmptyCells()
        {
            if (emptySprite != null)
            {
                for (int y = 0; y < height - header; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var squareClone = Instantiate(emptySprite, new Vector3(x, y, 0), Quaternion.identity);
                        squareClone.name = $"Board Space ( x = {x.ToString()}, y = {y.ToString()} )";
                        squareClone.transform.parent = this.transform;
                    }
                }
            }
            else
            {
                Debug.Log($"WARNING! Please assign the emptySprite object in the inspector");
            }
        }
    }
}