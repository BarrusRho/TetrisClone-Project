using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TetrisClone.Core
{
    public class ShapeHolder : MonoBehaviour
    {
        private float _shapeScale = 0.5f;
        
        public Transform holderTransform;
        public Shape holdShape = null;
        public bool canReleaseShape = false;

        public void CatchActiveShape(Shape activeShape)
        {
            if (holdShape)
            {
                Debug.LogWarning("ShapeHolder Warning: Release a shape before trying to hold");
            }

            if (!activeShape)
            {
                Debug.LogWarning("ShapeHolder Warning: Invalid Shape");
            }

            if (holderTransform)
            {
                activeShape.transform.position = holderTransform.position + activeShape.queueOffset;
                activeShape.transform.localScale = new Vector3(_shapeScale, _shapeScale, _shapeScale);
                holdShape = activeShape;
                activeShape.transform.rotation = Quaternion.identity;
            }
            else
            {
                Debug.LogWarning("ShapeHolder Warning: No transform assigned");
            }
        }

        public Shape ReleaseShape()
        {
            holdShape.transform.localScale = Vector3.one;
            Shape shape = holdShape;
            holdShape = null;
            canReleaseShape = false;
            return shape;
        }
    }
}
