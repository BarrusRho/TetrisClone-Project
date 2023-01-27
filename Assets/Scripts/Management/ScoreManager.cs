using System;
using TMPro;
using UnityEngine;

namespace TetrisClone.Management
{
    public class ScoreManager : MonoBehaviour
    {
        private int _score = 0;
        private int _lines;
        private int _level = 1;

        private const int _minLines = 1;
        private const int _maxLines = 4;

        public int linesPerLevel;

        private void Start()
        {
            ResetLevel();
        }

        public void ScoreLines(int numberOfLines)
        {
            numberOfLines = Mathf.Clamp(numberOfLines, _minLines, _maxLines);

            switch (numberOfLines)
            {
                case 1:
                    _score += 40 * _level;
                    break;
                case 2:
                    _score += 100 * _level;
                    break;
                case 3:
                    _score += 300 * _level;
                    break;
                case 4:
                    _score += 1200 * _level;
                    break;
            }
        }

        public void ResetLevel()
        {
            _level = 1;
            _lines = linesPerLevel * _level;
        }
    }
}
