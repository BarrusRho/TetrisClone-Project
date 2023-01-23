using System;
using UnityEngine;
using UnityEngine.UI;

namespace TetrisClone.Utility
{
    [RequireComponent(typeof(Image))]
    public class IconToggle : MonoBehaviour
    {
        public Sprite iconTrue;
        public Sprite iconFalse;

        public bool defaultIconState = true;

        private Image _image;

        private void Start()
        {
            _image = GetComponent<Image>();
            _image.sprite = defaultIconState ? iconTrue : iconFalse;
        }

        public void ToggleIcon(bool state)
        {
            if (!_image || !iconTrue || !iconFalse)
            {
                return;
            }

            _image.sprite = state ? iconTrue : iconFalse;
        }
    }
}
