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

        public static Vector3 WithX(this Vector3 value, float x)
        {
            return new Vector3(x, value.y, value.z);
        }

        public static Vector3 WithY(this Vector3 value, float y)
        {
            return new Vector3(value.x, y, value.z);
        }

        public static Vector3 WithZ(this Vector3 value, float z)
        {
            return new Vector3(value.x, value.y, z);
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

        public static float InverseLerpUnclamped(Vector3 a, Vector3 b, Vector3 value)
        {
            Vector3 AB = b - a;
            Vector3 AV = value - a;
            return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
        }

        public static Vector2 CalculateCenter(List<Vector2> points)
        {
            float area = 0f;
            float factor = 0f;
            Vector2 center = Vector2.zero;

            for (int i = 0; i < points.Count; i++)
            {
                Vector2 p0 = points[i];
                Vector2 p1 = points[(i + 1) % points.Count];

                factor = p0.x * p1.y - p1.x * p0.y;
                area += factor;
                center += new Vector2((p0.x + p1.x) * factor, (p0.y + p1.y) * factor);
            }

            area /= 2f;
            area *= 6f;
            factor = 1f / area;

            return new Vector2(center.x * factor, center.y * factor);
        }

        public static Vector3 GetRandomPointAtDistanceXZ(this Vector3 origin, float distance)
        {
            Vector2 randomPoint = GetRandomPointOnUnitCircleCircumference() * distance;
            return new Vector3(origin.x + randomPoint.x, origin.y, origin.z + randomPoint.y);
        }

        // Reference: https://stackoverflow.com/questions/9879258/how-can-i-generate-random-points-on-a-circles-circumference-in-javascript
        public static Vector2 GetRandomPointOnUnitCircleCircumference()
        {
            const int maximumAttempts = 30;
            var attempts = 0; // The expected number of attempts is ~1.27 until a valid point is found.

            do
            {
                float u = Random.value * 2f - 1f;
                float v = Random.value * 2f - 1f;

                float uSquared = u * u;
                float vSquared = v * v;
                float r = uSquared + vSquared;

                if (!(r > 1))
                {
                    return new Vector2((uSquared - vSquared) / r, (2f * u * v) / r);
                }

                ++attempts;
            } while (attempts < maximumAttempts);

            Debug.LogError("Failed to find a valid point! Returning Vector2.zero.");
            return Vector2.zero;
        }
    }
}