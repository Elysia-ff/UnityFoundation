using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.UI
{
    public class SafeArea : MonoBehaviour
    {
        private void OnEnable()
        {
            SafeAreaManager.Register(this);
        }

        private void OnDisable()
        {
            SafeAreaManager.Unregister(this);
        }

        public void Refresh(Vector2 anchorMin, Vector2 anchorMax)
        {
            RectTransform rectTransform = (RectTransform)transform;
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
        }
    }
}
