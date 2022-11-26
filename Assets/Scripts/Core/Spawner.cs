using UnityEngine;

namespace TetrisClone.Core
{
    public class Spawner : MonoBehaviour
    {
        public Shape[] allShapes;

        private Shape GetRandomShape()
        {
            var i = Random.Range(0, allShapes.Length);
            if (allShapes[i])
            {
                return allShapes[i];
            }
            else
            {
                Debug.Log($"WARNING! Invalid shape in Spawner");
                return null;
            }
        }

        public Shape SpawnShape()
        {
            Shape shape = null;
            shape = Instantiate(GetRandomShape(), transform.position, Quaternion.identity) as Shape;
            if (shape)
            {
                return shape;
            }
            else
            {
                Debug.Log($"WARNING! Invalid shape in Spawner");
                return null;
            }
        }
    }
}