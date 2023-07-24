using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public static class VectorExtensions
    {
        public static Vector2 XY(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.y);
        }

        public static Vector2 XZ(this Vector3 vector3)
        {
            return new Vector2(vector3.x, vector3.z);
        }

        public static Vector2 YZ(this Vector3 vector3)
        {
            return new Vector2(vector3.y, vector3.z);
        }
    }
}
