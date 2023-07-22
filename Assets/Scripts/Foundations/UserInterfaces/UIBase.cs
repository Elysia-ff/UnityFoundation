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

        protected virtual Vector2 Pivot => Vector2.zero;

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

        private void SetPosition(EPosition position)
        {
            switch (position)
            {
                case EPosition.CenterOfScreen:
                    Vector2 centerOfScreen = Game.Scene.UI.ScreenPointToUIPosition(new Vector2(Screen.width / 2, Screen.height / 2));
                    RectTransform.localPosition = centerOfScreen - Pivot;
                    break;

                case EPosition.CenterOfParent:
                    if (!HasParentUI)
                    {
                        goto case EPosition.CenterOfScreen;
                    }

                    Vector3 parentPosition = _parentUI.RectTransform.localPosition;
                    RectTransform.localPosition = parentPosition.XY() - Pivot;
                    break;

                case EPosition.PivotOfParent:
                    if (!HasParentUI)
                    {
                        goto case EPosition.CenterOfScreen;
                    }

                    Vector2 parentPivot = _parentUI.RectTransform.localPosition.XY() + _parentUI.Pivot;
                    RectTransform.localPosition = parentPivot - Pivot;
                    break;

                case EPosition.Pointer:
                    Vector2 screenPosition = Game.Scene.Input.GetMousePosition();
                    Vector2 pointer = Game.Scene.UI.ScreenPointToUIPosition(screenPosition);
                    RectTransform.localPosition = pointer - Pivot;
                    break;
            }
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
