using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TetrisClone.Management
{
    public class ScoreManager : MonoBehaviour
    {
        private int _score = 0;
        private int _lines;
        [FormerlySerializedAs("_level")] public int level = 1;

        private const int _minLines = 1;
        private const int _maxLines = 4;

        public int linesPerLevel;
        public bool hasLeveledUp = false;
        
        public TMP_Text linesText;
        public TMP_Text levelText;
        public TMP_Text scoreText;

        private void Start()
        {
            ResetLevel();
        }

        public void ScoreLines(int numberOfLines)
        {
            hasLeveledUp = false;
            
            numberOfLines = Mathf.Clamp(numberOfLines, _minLines, _maxLines);

            switch (numberOfLines)
            {
                case 1:
                    _score += 40 * level;
                    break;
                case 2:
                    _score += 100 * level;
                    break;
                case 3:
                    _score += 300 * level;
                    break;
                case 4:
                    _score += 1200 * level;
                    break;
            }

            _lines -= numberOfLines;

            if (_lines <= 0)
            {
                LevelUp();
            }
            
            UpdateUIText();
        }

        public void ResetLevel()
        {
            level = 1;
            _lines = linesPerLevel * level;
            UpdateUIText();
        }

        private void UpdateUIText()
        {
            if (linesText)
            {
                linesText.text = $"{_lines}";
            }

            if (levelText)
            {
                levelText.text = $"{level}";
            }

            if (scoreText)
            {
                scoreText.text = ScorePadding(_score, 4);
            }
        }

        private string ScorePadding(int number, int padDigits)
        {
            var paddedNumber = number.ToString();

            while (paddedNumber.Length < padDigits)
            {
                paddedNumber = $"0{paddedNumber}";
            }

            return paddedNumber;
        }

        public void LevelUp()
        {
            level++;
            _lines = linesPerLevel * level;
            hasLeveledUp = true;
        }
    }
}
