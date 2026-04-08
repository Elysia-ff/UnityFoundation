#if false

using Elysia.Pools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Elysia.UI
{
    public class MainCanvasExample : MonoBehaviour
    {
        public UIBase FocusedUI { get; private set; }
        public int UICount { get; private set; }

        public bool Interactable
        {
            get => _canvasGroup.interactable;
            set => _canvasGroup.interactable = value;
        }

        public bool BlocksRaycasts
        {
            get => _canvasGroup.blocksRaycasts;
            set => _canvasGroup.blocksRaycasts = value;
        }

        public RectTransform Container { get; private set; }
        private CanvasGroup _canvasGroup;

        private UIBase _globalModalWindow;

        private UIBase _movingUI;
        private Vector2 _movingUIDelta;
        private bool _hasUIMoveTag;

        private readonly Dictionary<Type, UIPool<UIBase>> _cachedUIs = new Dictionary<Type, UIPool<UIBase>>();

        private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();

        private const float SCREEN_MARGIN = 20f;

        private static readonly PointerEventData POINTER_EVENT = new PointerEventData(EventSystem.current);
        private static readonly string TAG_UI_MOVE = "UIMove";

        public void Initialize()
        {
            Container = (RectTransform)transform.Find("Container");
            _canvasGroup = Container.GetComponent<CanvasGroup>();

            try
            {
                GameObject _ = GameObject.FindGameObjectWithTag(TAG_UI_MOVE);
                _hasUIMoveTag = true;
            }
            catch
            {
                // swallow exception
            }

            App.Input.UIActions.Drag.started += OnDragStarted;
            App.Input.UIActions.Drag.performed += OnDragPerformed;
            App.Input.UIActions.Drag.canceled += OnDragCanceled;
        }

        private void OnDestroy()
        {
            App.Input.UIActions.Drag.started -= OnDragStarted;
            App.Input.UIActions.Drag.performed -= OnDragPerformed;
            App.Input.UIActions.Drag.canceled -= OnDragCanceled;
        }

        public T Show<T>(EPosition position, Action<T> beforeOnShow = null)
            where T : UIBase<T>
        {
            T ui = Get<T>();
            UIBase.InvokeSetIgnoreParentGroups(ui, false);
            UIBase.InvokeSetPosition(ui, position);

            beforeOnShow?.Invoke(ui);

            bool invokeFocusedEvent = Focus_Impl(ui);
            UIBase.InvokeEnable(ui);
            UICount++;

            if (invokeFocusedEvent)
            {
                UIBase.InvokeOnFocused(ui);
            }

            return ui;
        }

        public T ShowModal<T>(UIBase parent, EPosition position, Action<T> beforeOnShow = null)
            where T : UIBase<T>
        {
            T ui = Get<T>();

            if (parent == null)
            {
                Debug.Assert(_globalModalWindow == null, $"GlobalModalWindow already exists.");

                _globalModalWindow = ui;
                BlocksRaycasts = false;
                App.Scene.UI.HUD.BlocksRaycasts = false;
            }

            UIBase.InvokeSetIgnoreParentGroups(ui, true);
            UIBase.InvokeSetParent(ui, parent);
            UIBase.InvokeSetPosition(ui, position);

            beforeOnShow?.Invoke(ui);

            bool invokeFocusedEvent = Focus_Impl(ui);
            UIBase.InvokeEnable(ui);
            UICount++;

            if (invokeFocusedEvent)
            {
                UIBase.InvokeOnFocused(ui);
            }

            return ui;
        }

        [Obsolete("Do not call this method directly. Use UIBase.Close() instead.")]
        public void InvokeHide<T>(UIBase ui)
            where T : UIBase
        {
            Type t = typeof(T);
            Debug.Assert(_cachedUIs.ContainsKey(t));

            if (FocusedUI == ui)
            {
                FocusedUI = null;
            }

            if (_globalModalWindow == ui)
            {
                _globalModalWindow = null;
                BlocksRaycasts = true;
                App.Scene.UI.HUD.BlocksRaycasts = true;
            }

            UIBase.InvokeDisable(ui);
            UICount--;

            _cachedUIs[t].Release(ref ui);
        }

        public void Focus(UIBase ui)
        {
            if (Focus_Impl(ui))
            {
                UIBase.InvokeOnFocused(FocusedUI);
            }
        }

        private bool Focus_Impl(UIBase ui)
        {
            if (FocusedUI == ui)
            {
                return false;
            }

            StopMovingUI();

            FocusedUI = ui;

            if (FocusedUI != null)
            {
                FocusedUI.RectTransform.SetAsLastSibling();
                return true;
            }

            return false;
        }

        private T Get<T>()
            where T : UIBase
        {
            Type t = typeof(T);
            if (!_cachedUIs.TryGetValue(t, out UIPool<UIBase> pool))
            {
                pool = new UIPool<UIBase>(Create<T>, 0, 4);
                _cachedUIs.Add(t, pool);
            }

            return (T)pool.Get();
        }

        private UIBase Create<T>()
            where T : UIBase
        {
            T prefab = App.ResourceHolder.GetUI<T>();
            if (prefab == null)
            {
                Type t = typeof(T);
                string key = $"UI/Prefabs/{t.Name}";
                prefab = Resources.Load<T>(key);
                Debug.Assert(prefab != null, $"Not found '{key}'");
            }

            UIBase ui = Instantiate(prefab, Container, false);
            UIBase.InvokeInitialize(ui);
            return ui;
        }

        private void OnDragStarted(InputAction.CallbackContext context)
        {
            Vector2 position = context.ReadValue<Vector2>();
            POINTER_EVENT.position = position;
            EventSystem.current.RaycastAll(POINTER_EVENT, _raycastResults);

            if (_raycastResults.Count > 0)
            {
                RaycastResult result = _raycastResults[0];
                UIBase ui = result.gameObject.GetComponentInParent<UIBase>();

                if (ui != null && (_globalModalWindow == null || _globalModalWindow == ui))
                {
                    Focus(ui);
                    UIBase.InvokeOnPointerPressed(ui);

                    if (_hasUIMoveTag && result.gameObject.CompareTag(TAG_UI_MOVE))
                    {
                        Vector2 uiPosition = App.Scene.UI.ScreenPointToUIPosition(position);
                        Vector2 delta = uiPosition - ui.RectTransform.localPosition.XY();

                        _movingUI = ui;
                        _movingUIDelta = delta;
                    }
                }

                _raycastResults.Clear();
            }
        }

        private void OnDragPerformed(InputAction.CallbackContext context)
        {
            if (_movingUI == null)
            {
                return;
            }

            Vector2 position = context.ReadValue<Vector2>();
            position.x = Mathf.Clamp(position.x, SCREEN_MARGIN, Screen.width - SCREEN_MARGIN);
            position.y = Mathf.Clamp(position.y, SCREEN_MARGIN, Screen.height - SCREEN_MARGIN);

            Vector2 uiPosition = App.Scene.UI.ScreenPointToUIPosition(position);
            _movingUI.RectTransform.localPosition = uiPosition - _movingUIDelta;
        }

        private void OnDragCanceled(InputAction.CallbackContext context)
        {
            StopMovingUI();
        }

        private void StopMovingUI()
        {
            _movingUI = null;
        }
    }
}

#endif
