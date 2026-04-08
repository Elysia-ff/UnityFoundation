using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.UI
{
    public static class SafeAreaManager
    {
        public static bool IsPortrait { get; private set; }

        private static readonly List<SafeArea> _safeAreas = new List<SafeArea>();
        private static Vector2Int _screenSize;
        private static Vector2 _anchorMin;
        private static Vector2 _anchorMax;

        public static event System.Action OnOrientationChanged;

        static SafeAreaManager()
        {
            UpdateProperties();
        }

        public static void OnUpdate()
        {
            if (_screenSize.x != Screen.width || _screenSize.y != Screen.height)
            {
                UpdateProperties();

                for (int i = 0; i < _safeAreas.Count; i++)
                {
                    _safeAreas[i].Refresh(_anchorMin, _anchorMax);
                }
            }
        }

        public static void Register(SafeArea safeArea)
        {
            Debug.Assert(!_safeAreas.Contains(safeArea));

            safeArea.Refresh(_anchorMin, _anchorMax);
            _safeAreas.Add(safeArea);
        }

        public static void Unregister(SafeArea safeArea)
        {
            Debug.Assert(_safeAreas.Contains(safeArea));

            _safeAreas.Remove(safeArea);
        }

        private static void UpdateProperties()
        {
            _screenSize.x = Screen.width;
            _screenSize.y = Screen.height;

            bool prevIsPortrait = IsPortrait;
            IsPortrait = _screenSize.x < _screenSize.y;
            if (prevIsPortrait != IsPortrait)
            {
                OnOrientationChanged?.Invoke();
            }

            Rect safeArea = Screen.safeArea;
            Vector2 minAnchor = safeArea.position;
            Vector2 maxAnchor = minAnchor + safeArea.size;

            minAnchor.x /= Screen.width;
            minAnchor.y /= Screen.height;
            maxAnchor.x /= Screen.width;
            maxAnchor.y /= Screen.height;

            _anchorMin = minAnchor;
            _anchorMax = maxAnchor;
        }
    }
}
