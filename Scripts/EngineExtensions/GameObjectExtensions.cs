using UnityEngine;

namespace Elysia
{
    public static class GameObjectExtensions
    {
        public static void SetActiveSafe(this GameObject gameObject, bool value)
        {
            if (gameObject.activeSelf == value)
            {
                return;
            }

            gameObject.SetActive(value);
        }

        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            if (!gameObject.TryGetComponent<T>(out T component))
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }
    }
}