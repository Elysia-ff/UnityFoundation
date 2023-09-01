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

        public static Vector2Int XY(this Vector3Int vector3)
        {
            return new Vector2Int(vector3.x, vector3.y);
        }

        public static Vector2Int XZ(this Vector3Int vector3)
        {
            return new Vector2Int(vector3.x, vector3.z);
        }

        public static Vector2Int YZ(this Vector3Int vector3)
        {
            return new Vector2Int(vector3.y, vector3.z);
        }

        public static Vector2 QuadraticBezierCurve(Vector2 p0, Vector2 p1, Vector2 p2, float t)
        {
            Vector2 m0 = Vector2.Lerp(p0, p1, t);
            Vector2 m1 = Vector2.Lerp(p1, p2, t);

            return Vector2.Lerp(m0, m1, t);
        }

        public static Vector3 QuadraticBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            Vector3 m0 = Vector3.Lerp(p0, p1, t);
            Vector3 m1 = Vector3.Lerp(p1, p2, t);

            return Vector3.Lerp(m0, m1, t);
        }

        public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
        {
            Vector3 AB = b - a;
            Vector3 AV = value - a;
            return Mathf.Clamp01(Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB));
        }
    }
}
