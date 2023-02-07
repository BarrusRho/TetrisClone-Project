using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TetrisClone.Management
{
    public class TouchManager : MonoBehaviour
    {
        public delegate void TouchEventHandler(Vector2 swipe);

        public static event TouchEventHandler DragEvent;
        public static event TouchEventHandler SwipeEvent;
        public static event TouchEventHandler TapEvent;

        public TMP_Text diagnosticText1;
        public TMP_Text diagnosticText2;
        public bool canUseDiagnostic = false;

        private Vector2 _touchMovement;
        [Range(50, 150)] public int minimumDragDistance = 100;
        [Range(50, 250)] public int minimumSwipeDistance = 200;
        private float _tapTimeMaximum = 0f;
        public float tapTimeWindow = 0.1f;

        private void Start()
        {
            DisplayDiagnostic("", "");
        }

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                var touch = Input.touches[0];

                if (touch.phase == TouchPhase.Began)
                {
                    _touchMovement = Vector2.zero;
                    _tapTimeMaximum = Time.time + tapTimeWindow;
                    DisplayDiagnostic("", "");
                }
                else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    _touchMovement += touch.deltaPosition;

                    if (_touchMovement.magnitude > minimumDragDistance)
                    {
                        OnDrag();
                        DisplayDiagnostic($"Drag detected",
                            $"{_touchMovement.ToString()} {SwipeDiagnostic(_touchMovement)}");
                    }
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    if (_touchMovement.magnitude > minimumSwipeDistance)
                    {
                        OnSwipeEnd();
                        DisplayDiagnostic($"Swipe detected",
                            $"{_touchMovement.ToString()} {SwipeDiagnostic(_touchMovement)}");
                    }
                    else if (Time.time < _tapTimeMaximum)
                    {
                        OnTap();
                        DisplayDiagnostic($"Tap detected",
                            $"{_touchMovement.ToString()} {SwipeDiagnostic(_touchMovement)}");
                    }
                }
            }
        }

        private void OnDrag()
        {
            if (DragEvent != null)
            {
                DragEvent(_touchMovement);
            }
        }

        private void OnSwipeEnd()
        {
            if (SwipeEvent != null)
            {
                SwipeEvent(_touchMovement);
            }
        }

        private void OnTap()
        {
            if (TapEvent != null)
            {
                TapEvent(_touchMovement);
            }
        }

        private void DisplayDiagnostic(string text1, string text2)
        {
            diagnosticText1.gameObject.SetActive(canUseDiagnostic);
            diagnosticText2.gameObject.SetActive(canUseDiagnostic);

            if (diagnosticText1 && diagnosticText2)
            {
                diagnosticText1.text = text1;
                diagnosticText2.text = text2;
            }
        }

        private String SwipeDiagnostic(Vector2 swipeMovement)
        {
            var direction = "";

            if (Mathf.Abs(swipeMovement.x) > Mathf.Abs(swipeMovement.y))
            {
                direction = (swipeMovement.x >= 0) ? "right" : "left";
            }
            else
            {
                direction = (swipeMovement.y >= 0) ? "up" : "down";
            }

            return direction;
        }
    }
}