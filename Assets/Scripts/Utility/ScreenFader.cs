using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TetrisClone.Utility
{
    [RequireComponent(typeof(MaskableGraphic))]
    public class ScreenFader : MonoBehaviour
    {
        private float _increment;
        private float _currentAlpha;
        private MaskableGraphic _graphic;
        private Color _originalColour;
        
        public float startAlpha = 1f;
        public float targetAlpha = 0;
        public float startDelay = 0f;
        public float timeToFade = 1f;

        private void Awake()
        {
            _graphic = GetComponent<MaskableGraphic>();
        }

        private void Start()
        {
            _originalColour = _graphic.color;
            _currentAlpha = startAlpha;
            var tempColour = new Color(_originalColour.r, _originalColour.g, _originalColour.b, _currentAlpha);
            _graphic.color = tempColour;
            _increment = ((targetAlpha - startAlpha) / timeToFade) * Time.deltaTime;
            StartCoroutine(nameof(ScreenFadeRoutine));
        }

        private IEnumerator ScreenFadeRoutine()
        {
            yield return new WaitForSeconds(startDelay);

            while (Mathf.Abs(targetAlpha - _currentAlpha) > 0.01f)
            {
                yield return new WaitForEndOfFrame();
                _currentAlpha += _increment;
                
                var tempColour = new Color(_originalColour.r, _originalColour.g, _originalColour.b, _currentAlpha);
                _graphic.color = tempColour;
            }
            
            Debug.Log($"Screen Fader complete");
        }
    }
}