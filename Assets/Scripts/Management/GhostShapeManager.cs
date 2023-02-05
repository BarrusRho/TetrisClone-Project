using TetrisClone.Core;
using UnityEngine;

namespace TetrisClone.Management
{
    public class GhostShapeManager : MonoBehaviour
    {
        private Shape _ghostShape = null;
        private bool _hasHitBottom = false;

        public Color shapeColour = new Color(1f, 1f, 1f, 0.2f);

        public void DrawGhostShape(Shape originalShape, Board gameBoard)
        {
            if (!_ghostShape)
            {
                _ghostShape = Instantiate(originalShape, originalShape.transform.position,
                    originalShape.transform.rotation) as Shape;
                _ghostShape.gameObject.name = $"GhostShape";

                var allRenderers = _ghostShape.GetComponentsInChildren<SpriteRenderer>();
                foreach (var renderer in allRenderers)
                {
                    renderer.color = shapeColour;
                }
            }
            else
            {
                _ghostShape.transform.position = originalShape.transform.position;
                _ghostShape.transform.rotation = originalShape.transform.rotation;
                _ghostShape.transform.localScale = Vector3.one;
            }

            _hasHitBottom = false;

            while (!_hasHitBottom)
            {
                _ghostShape.MoveDown();

                if (!gameBoard.IsValidPosition(_ghostShape))
                {
                    _ghostShape.MoveUp();
                    _hasHitBottom = true;
                }
            }
        }

        public void ResetGhostShape()
        {
            Destroy(_ghostShape.gameObject);
        }
    }
}
