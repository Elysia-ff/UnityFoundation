using System.Collections;
using System.Collections.Generic;
using Elysia;
using UnityEngine;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(LayoutElement))]
    public abstract class LoopScrollElement : Elysia.UI.UIElement
    {
        public RectTransform RectTransform { get; private set; }
        private CanvasGroup _canvasGroup;
        private LayoutElement _layoutElement;

        [field: SerializeField, ReadOnly] public float PreferredWidth { get; private set; }
        [field: SerializeField, ReadOnly] public float PreferredHeight { get; private set; }

        public virtual void Initialize()
        {
            RectTransform = (RectTransform)transform;
            _canvasGroup = GetComponent<CanvasGroup>();
            _layoutElement = GetComponent<LayoutElement>();

            CalculatePreferredWidth();
            CalculatePreferredHeight();
        }

        public void Activate()
        {
            _layoutElement.ignoreLayout = false;
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;

            OnActivated();
        }

        protected virtual void OnActivated()
        {
        }

        public void Deactivate()
        {
            _layoutElement.ignoreLayout = true;
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;

            OnDeactivated();
        }

        protected virtual void OnDeactivated()
        {
        }

        public virtual void UpdateUI(object context, int index)
        {
        }

        protected void SetPreferredWidth(float width)
        {
            _layoutElement.preferredWidth = width;
            PreferredWidth = width;
        }

        private void CalculatePreferredWidth()
        {
            float minWidth = LayoutUtility.GetLayoutProperty(RectTransform, e => e.minWidth, 0, out ILayoutElement minLayoutElement);
            float preferredWidth = LayoutUtility.GetLayoutProperty(RectTransform, e => e.preferredWidth, 0, out ILayoutElement preferredLayoutElement);

            if (preferredLayoutElement == null && minLayoutElement == null)
            {
                PreferredWidth = RectTransform.rect.width;
            }
            else
            {
                PreferredWidth = Mathf.Max(minWidth, preferredWidth);
            }
        }

        protected void SetPreferredHeight(float height)
        {
            _layoutElement.preferredHeight = height;
            PreferredHeight = height;
        }

        private void CalculatePreferredHeight()
        {
            float minHeight = LayoutUtility.GetLayoutProperty(RectTransform, e => e.minHeight, 0, out ILayoutElement minLayoutElement);
            float preferredHeight = LayoutUtility.GetLayoutProperty(RectTransform, e => e.preferredHeight, 0, out ILayoutElement preferredLayoutElement);

            if (preferredLayoutElement == null && minLayoutElement == null)
            {
                PreferredHeight = RectTransform.rect.height;
            }
            else
            {
                PreferredHeight = Mathf.Max(minHeight, preferredHeight);
            }
        }
    }
}
