using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TetrisClone.Utility
{
    public static class Vectorf
    {
        public static Vector2 Round(Vector2 vector)
        {
            return new Vector2(Mathf.Round(vector.x), Mathf.Round(vector.y));
        }

        public static Vector3 Round(Vector3 vector)
        {
            return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
        }
    }
}
