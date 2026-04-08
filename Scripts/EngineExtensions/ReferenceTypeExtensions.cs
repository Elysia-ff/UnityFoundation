using UnityEngine;

namespace Elysia
{
    public static class ReferenceTypeExtensions
    {
        public static T GetSelfOrNull<T>(this T referenceTypeObject) where T : class
        {
            if (referenceTypeObject is Object unityObject && !unityObject)
            {
                return null;
            }

            return referenceTypeObject;
        }

        public static bool IsNull<T>(this T referenceTypeObject) where T : class
        {
            if (referenceTypeObject is Object unityObject)
            {
                return !unityObject;
            }

            return referenceTypeObject == null;
        }
    }
}