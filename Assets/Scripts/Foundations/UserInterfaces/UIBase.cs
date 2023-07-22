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

        public bool Interactable
        {
            get => _canvasGroup.interactable;
            set
            {
                _canvasGroup.interactable = value;
                OnInteractableChanged(value);
            }
        }

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

        public abstract void Close();

        protected virtual void OnFocused()
        {
        }

        protected virtual void OnInteractableChanged(bool value)
        {
        }

        protected virtual void OnPointerPressed()
        {
            Debug.Log(Interactable);
        }
    }

    public abstract class UIBase<T> : UIBase
        where T : UIBase<T>
    {
        public sealed override void Close()
        {
            Game.Scene.UI.HideUI<T>(this);
        }
    }
}
