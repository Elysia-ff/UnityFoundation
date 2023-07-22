using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract partial class UIBase : MonoBehaviour
    {
        public RectTransform RectTransform { get; private set; }

        private CanvasGroup _canvasGroup;

        private UIBase _parentUI;
        public bool HasParentUI => _parentUI != null;

        public bool Interactable => _canvasGroup.interactable;

        public float Alpha
        {
            get => _canvasGroup.alpha;
            set => _canvasGroup.alpha = value;
        }

        protected virtual void Initialize()
        {
            RectTransform = (RectTransform)transform;
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected void SetParent(UIBase parent)
        {
            _parentUI = parent;
            parent._canvasGroup.interactable = false;
        }

        public virtual void Close()
        {
            if (_parentUI != null)
            {
                _parentUI._canvasGroup.interactable = true;
                _parentUI = null;
            }
        }

        protected virtual void OnFocused()
        {
        }

        protected virtual void OnInteractableChanged(bool value)
        {
        }

        protected virtual void OnPointerPressed()
        {
        }
    }

    public abstract class UIBase<T> : UIBase
        where T : UIBase<T>
    {
        public sealed override void Close()
        {
            base.Close();

            Game.Scene.UI.HideUI<T>(this);
        }
    }
}
