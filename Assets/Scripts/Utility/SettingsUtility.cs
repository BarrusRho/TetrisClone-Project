using System;
using TetrisClone.Management;
using UnityEngine;
using UnityEngine.UI;

namespace TetrisClone.Utility
{
    public class SettingsUtility : MonoBehaviour
    {
        private GameManager _gameManager;
        private TouchManager _touchManager;

        public Slider dragDistanceSlider;
        public Slider swipeDistanceSlider;
        public Slider dragTimeSlider;
        public Toggle diagnosticsToggle;

        private void Awake()
        {
            _gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
            _touchManager = FindObjectOfType<TouchManager>().GetComponent<TouchManager>();
        }

        private void Start()
        {
            if (dragDistanceSlider != null)
            {
                dragDistanceSlider.value = 100;
                dragDistanceSlider.minValue = 50;
                dragDistanceSlider.maxValue = 150;
            }

            if (swipeDistanceSlider != null)
            {
                swipeDistanceSlider.value = 50;
                swipeDistanceSlider.minValue = 20;
                swipeDistanceSlider.maxValue = 250;
            }

            if (dragTimeSlider != null)
            {
                dragDistanceSlider.value = 0.15f;
                dragDistanceSlider.minValue = 0.05f;
                dragDistanceSlider.maxValue = 0.5f;
            }

            if (diagnosticsToggle != null && _touchManager != null)
            {
                _touchManager.canUseDiagnostic = diagnosticsToggle.isOn;
            }
        }

        public void UpdateSettingsPanel()
        {
            if (dragDistanceSlider != null && _touchManager != null)
            {
                _touchManager.minimumDragDistance = (int)dragDistanceSlider.value;
            }

            if (swipeDistanceSlider != null && _touchManager != null)
            {
                _touchManager.minimumSwipeDistance = (int)swipeDistanceSlider.value;
            }

            if (dragTimeSlider != null && _gameManager != null)
            {
                _gameManager.minimumTimeToDrag = dragTimeSlider.value;
            }
            
            if (diagnosticsToggle != null && _touchManager != null)
            {
                _touchManager.canUseDiagnostic = diagnosticsToggle.isOn;
            }
        }
    }
}