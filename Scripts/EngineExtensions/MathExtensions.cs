using UnityEngine;

namespace Elysia
{
    public static class MathExtensions
    {
        public static float InverseLerpUnclamped(float a, float b, float value)
        {
            return (float)(((double)value - (double)a) / ((double)b - (double)a));
        }
    }
}
