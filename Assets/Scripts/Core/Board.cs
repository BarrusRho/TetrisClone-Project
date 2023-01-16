using System;
using System.Numerics;
using TetrisClone.Utility;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace TetrisClone.Core
{
    public class Board : MonoBehaviour
    {
        public Transform emptySprite;
        public int height = 30;
        public int width = 10;
        public int header = 8;
        public int completedRows = 0;

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
                Debug.Log($"WARNING! Please assign the emptySprite object in inspector");
            }
        }

        private bool IsWithinBoard(int x, int y)
        {
            return (x >= 0 && x < width && y >= 0);
        }

        public bool IsValidPosition(Shape shape)
        {
            foreach (Transform child in shape.transform)
            {
                Vector2 position = Vectorf.Round(child.position);

                if (!IsWithinBoard((int)position.x, (int)position.y))
                {
                    return false;
                }

                if (IsOccupied((int)position.x, (int)position.y, shape))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsOccupied(int x, int y, Shape shape)
        {
            return (_grid[x, y] != null && _grid[x, y].parent != shape.transform);
        }

        public void StoreShapeInGrid(Shape shape)
        {
            if (shape == null)
            {
                return;
            }

            foreach (Transform child in shape.transform)
            {
                Vector2 position = Vectorf.Round(child.position);
                _grid[(int)position.x, (int)position.y] = child;
            }
        }

        private bool IsComplete(int y)
        {
            for (int x = 0; x < width; x++)
            {
                if (_grid[x, y] == null)
                {
                    return false;
                }
            }

            return true;
        }

        private void ClearRow(int y)
        {
            for (int x = 0; x < width; x++)
            {
                if (_grid[x, y] != null)
                {
                    Destroy(_grid[x, y].gameObject);
                }

                _grid[x, y] = null;
            }
        }

        private void ShiftOneRowDown(int y)
        {
            for (int x = 0; x < width; x++)
            {
                if (_grid[x, y] != null)
                {
                    _grid[x, y - 1] = _grid[x, y];
                    _grid[x, y] = null;
                    _grid[x, y - 1].position += new Vector3(0, -1, 0);
                }
            }
        }

        private void ShiftRowsDown(int startY)
        {
            for (int i = startY; i < height; i++)
            {
                ShiftOneRowDown(i);
            }
        }

        public void ClearAllRows()
        {
            completedRows = 0;
            
            for (int y = 0; y < height; y++)
            {
                if (IsComplete(y))
                {
                    completedRows++;
                    ClearRow(y);
                    ShiftRowsDown(y + 1);
                    y--;
                }
            }
        }

        public bool IsOverLimit(Shape shape)
        {
            foreach (Transform child in shape.transform)
            {
                if (child.transform.position.y >= (height - header) - 1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}