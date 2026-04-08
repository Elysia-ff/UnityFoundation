using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.UI
{
    public partial class UIManager : MonoBehaviour
    {
        [SerializeField][ReadOnlyInPlay] private Camera _camera;
        public Camera Camera => _camera;

        public void Initialize()
        {
            InitializeCanvases();
        }

        public Vector2 ScreenPointToUIPosition(Vector2 screenPosition)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(Main.Container, screenPosition, Camera, out Vector2 localPosition))
            {
                return localPosition;
            }

            return Vector2.zero;
        }

        private void Update()
        {
            SafeAreaManager.OnUpdate();
        }
    }
}
