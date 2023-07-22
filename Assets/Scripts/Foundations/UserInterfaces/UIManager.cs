using Elysia.Inputs;
using Elysia.Pools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elysia.UI
{
    public class UIManager : MonoBehaviour
    {
        public Camera Camera { get; private set; }

        public UIBase FocusedUI { get; private set; }

        private RectTransform _windowContainer;
        private UIBase _movingUI;
        private Vector2 _movingUIDelta;
        private int _uiMoveLayer;

        private readonly Dictionary<Type, UIBase> _uiPrefabs = new Dictionary<Type, UIBase>();
        private readonly Dictionary<Type, IObjectPool<UIBase>> _cachedUIs = new Dictionary<Type, IObjectPool<UIBase>>();

        private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();

        private const float SCREEN_MARGIN = 20f;

        public UIManager Initialize()
        {
            Camera = transform.Find("Camera").GetComponent<Camera>();
            _windowContainer = (RectTransform)transform.Find("WindowCanvas/Container");
            _uiMoveLayer = LayerMask.NameToLayer("UI Move");

            Game.Scene.Input.BindMouse(0, EInputType.Pressed, OnLMBPressed);
            Game.Scene.Input.BindMouse(0, EInputType.Holding, OnLMBHolding);
            Game.Scene.Input.BindMouse(0, EInputType.Released, OnLMBReleased);

            return this;
        }

        public T ShowUI<T>()
            where T : UIBase
        {
            Type t = typeof(T);
            if (!_cachedUIs.TryGetValue(t, out IObjectPool<UIBase> pool))
            {
                pool = new ObjectPool<UIBase>(_windowContainer, CreateUI<T>, null, false, 0, 4);
                _cachedUIs.Add(t, pool);
            }

            UIBase ui = pool.Get();
            FocusUI(ui);

            return (T)ui;
        }

        public T ShowModalUI<T>(UIBase parent)
            where T : UIBase
        {
            T ui = ShowUI<T>();
            UIBase.InvokeSetParent(ui, parent);

            return ui;
        }

        public void HideUI<T>(UIBase ui)
            where T : UIBase
        {
            Type t = typeof(T);
            Debug.Assert(_cachedUIs.ContainsKey(t));

            _cachedUIs[t].Release(ref ui);
        }

        public void FocusUI(UIBase ui)
        {
            if (FocusedUI == ui || !ui.Interactable)
            {
                return;
            }

            _movingUI = null;

            FocusedUI = ui;

            if (FocusedUI != null)
            {
                FocusedUI.RectTransform.SetAsLastSibling();
                UIBase.InvokeOnFocused(FocusedUI);
            }
        }

        private UIBase CreateUI<T>()
            where T : UIBase
        {
            Type t = typeof(T);
            if (!_uiPrefabs.TryGetValue(t, out UIBase prefab))
            {
                prefab = Resources.Load<T>($"Prefabs/UI/{t.Name}");
                Debug.Assert(prefab != null);

                _uiPrefabs.Add(t, prefab);
            }

            UIBase ui = Instantiate(prefab);
            UIBase.InvokeInitialize(ui);
            return ui;
        }

        private void OnLMBPressed(Vector2 position, EModifier modifier)
        {
            PointerEventData data = new PointerEventData(EventSystem.current);
            data.position = position;

            EventSystem.current.RaycastAll(data, _raycastResults);

            if (_raycastResults.Count > 0)
            {
                RaycastResult result = _raycastResults[0];
                UIBase ui = result.gameObject.GetComponentInParent<UIBase>();

                if (ui.Interactable)
                {
                    FocusUI(ui);
                    UIBase.InvokeOnPointerPressed(ui);

                    if (result.gameObject.layer == _uiMoveLayer)
                    {
                        _movingUI = ui;
                        Vector2 uiPosition = ScreenPointToUIPosition(position);
                        _movingUIDelta = uiPosition - ui.RectTransform.localPosition.XY();
                    }
                }
            }
        }

        private void OnLMBHolding(Vector2 position, EModifier modifier)
        {
            if (!_movingUI)
            {
                return;
            }

            position.x = Mathf.Clamp(position.x, SCREEN_MARGIN, Screen.width - SCREEN_MARGIN);
            position.y = Mathf.Clamp(position.y, SCREEN_MARGIN, Screen.height - SCREEN_MARGIN);

            Vector2 uiPosition = ScreenPointToUIPosition(position);
            _movingUI.RectTransform.localPosition = uiPosition - _movingUIDelta;
        }

        private void OnLMBReleased(Vector2 position, EModifier modifier)
        {
            _movingUI = null;
        }

        public Vector2 ScreenPointToUIPosition(Vector2 screenPosition)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_windowContainer, screenPosition, Camera, out Vector2 localPosition))
            {
                return localPosition;
            }

            return Vector2.zero;
        }
    }
}
