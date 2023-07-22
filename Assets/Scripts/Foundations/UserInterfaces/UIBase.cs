using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Elysia.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIBase : MonoBehaviour
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

        public virtual void Initialize()
        {
            RectTransform = (RectTransform)transform;
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public abstract void Close();

        public virtual void OnFocused()
        {
        }

        protected virtual void OnInteractableChanged(bool value)
        {
        }

        public virtual void OnPointerPressed()
        {
            Debug.Log(Interactable);
        }
    }

    public class UIBase<T> : UIBase
        where T : UIBase<T>
    {
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Close()
        {
            Game.Scene.UI.HideUI<T>(this);
        }
    }
}
