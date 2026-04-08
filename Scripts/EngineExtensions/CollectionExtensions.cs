using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Performs the Fisher–Yates shuffle. This is a mutable method.
        /// </summary>
        public static void Shuffle<T>(this IList<T> values)
        {
            int count = values.Count;
            for (int i = count - 1; i > 0; i--) 
            {
                int k = Random.Range(0, i + 1);
                (values[k], values[i]) = (values[i], values[k]);
            }
        }
    }
}