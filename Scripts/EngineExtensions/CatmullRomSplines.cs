using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public static class CatmullRomSplines
    {
        public readonly struct Builder
        {
            private readonly Vector2 segA;
            private readonly Vector2 segB;
            private readonly Vector2 segC;
            private readonly Vector2 segD;

            public Builder(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float alpha, float tension)
            {
                alpha = Mathf.Clamp01(alpha);
                tension = Mathf.Clamp01(tension);

                float t01 = Mathf.Pow(Vector2.Distance(p0, p1), alpha);
                float t12 = Mathf.Pow(Vector2.Distance(p1, p2), alpha);
                float t23 = Mathf.Pow(Vector2.Distance(p2, p3), alpha);

                Vector2 m1 = (1.0f - tension) * (p2 - p1 + t12 * ((p1 - p0) / t01 - (p2 - p0) / (t01 + t12)));
                Vector2 m2 = (1.0f - tension) * (p2 - p1 + t12 * ((p3 - p2) / t23 - (p3 - p1) / (t12 + t23)));

                segA = 2.0f * (p1 - p2) + m1 + m2;
                segB = -3.0f * (p1 - p2) - m1 - m1 - m2;
                segC = m1;
                segD = p1;
            }

            public Vector2 Simulate(float t)
            {
                Vector2 point = t * t * t * segA +
                                t * t * segB +
                                t * segC +
                                segD;

                return point;
            }
        }

        // https://qroph.github.io/2018/07/30/smooth-paths-using-catmull-rom-splines.html
        public static Builder MakeBuilder(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float alpha, float tension)
        {
            return new Builder(p0, p1, p2, p3, alpha, tension);
        }
    }
}
