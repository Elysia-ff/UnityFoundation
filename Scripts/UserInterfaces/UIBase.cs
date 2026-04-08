using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.UI
{
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasGroup))]
    public abstract partial class UIBase : UIContainer
    {
        public RectTransform RectTransform { get; private set; }

        private IRootCanvas _rootCanvas;

        private Canvas _canvas;
        public int SortingOrder => _canvas.sortingOrder;

        private CanvasGroup _canvasGroup;
        public bool Interactable => _canvasGroup.interactable;

        private UIBase _parentUI;
        public bool HasParentUI => _parentUI != null;

        public bool UIEnabled { get; private set; } = true;

        protected virtual Vector2 Pivot => Vector2.zero;

        protected virtual void Initialize()
        {
            RectTransform = (RectTransform)transform;
            _canvas = GetComponent<Canvas>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void SetRootCanvas(IRootCanvas rootCanvas)
        {
            _rootCanvas = rootCanvas;
        }

        private void Enable()
        {
            UIEnabled = true;
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;

            OnShow();
        }

        private void Disable()
        {
            OnHide();

            UIEnabled = false;
            _canvasGroup.alpha = 0f;
            _canvasGroup.blocksRaycasts = false;
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        public virtual void Close()
        {
#pragma warning disable CS0618
            _rootCanvas.InvokeHide(this);
#pragma warning restore CS0618

            if (_parentUI != null)
            {
                _parentUI._canvasGroup.blocksRaycasts = true;
                _parentUI = null;
            }
        }

        private void SetParent(UIBase parent)
        {
            _parentUI = parent;

            if (parent != null)
            {
                parent._canvasGroup.blocksRaycasts = false;
            }
        }

        protected virtual void OnFocused()
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
                    Vector2 centerOfScreen = App.Scene.UI.ScreenPointToUIPosition(new Vector2(Screen.width / 2, Screen.height / 2));
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
                    Vector2 screenPosition = App.Input.UIActions.Point.ReadValue<Vector2>();
                    Vector2 pointer = App.Scene.UI.ScreenPointToUIPosition(screenPosition);
                    RectTransform.localPosition = pointer - Pivot;
                    break;
            }
        }

        private void SetIgnoreParentGroups(bool value)
        {
            _canvasGroup.ignoreParentGroups = value;
        }
    }

    public abstract class AnimatedUIBase : UIBase
    {
        private Animator _animator;

        public static readonly int ID_SHOW = Animator.StringToHash("ID_SHOW");
        public static readonly int ID_HIDE = Animator.StringToHash("ID_HIDE");

        protected override void Initialize()
        {
            base.Initialize();

            _animator = GetComponent<Animator>();
        }

        protected override void OnShow()
        {
            base.OnShow();

            transform.localScale = Vector3.zero;
            _animator.Play(ID_SHOW, -1, 0f);
        }

        protected void PlayHideAnimation()
        {
            _animator.Play(ID_HIDE, -1, 0f);
        }
    }
}
