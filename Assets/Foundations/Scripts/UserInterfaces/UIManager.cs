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
        private CanvasGroup _windowCanvasGroup;
        private RectTransform _hudContainer;
        private CanvasGroup _hudCanvasGroup;
        private UIBase _globalModelWindow;

        private UIBase _movingUI;
        private Vector2 _movingUIDelta;
        private int _uiMoveLayer;

        private readonly Dictionary<Type, UIBase> _uiPrefabs = new Dictionary<Type, UIBase>();
        private readonly Dictionary<Type, IObjectPool<UIBase>> _cachedUIs = new Dictionary<Type, IObjectPool<UIBase>>();

        private readonly Dictionary<Type, HUDBase> _cachedHUDs = new Dictionary<Type, HUDBase>();

        private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();

        private const float SCREEN_MARGIN = 20f;

        public UIManager Initialize()
        {
            Camera = transform.Find("Camera").GetComponent<Camera>();

            _windowContainer = (RectTransform)transform.Find("WindowCanvas/Container");
            _windowCanvasGroup = _windowContainer.GetComponent<CanvasGroup>();
            _hudContainer = (RectTransform)transform.Find("HUDCanvas/Container");
            _hudCanvasGroup = _hudContainer.GetComponent<CanvasGroup>();

            _uiMoveLayer = LayerMask.NameToLayer("UI Move");

            Game.Scene.Input.BindMouse(0, EInputType.Pressed, OnLMBPressed);
            Game.Scene.Input.BindMouse(0, EInputType.Holding, OnLMBHolding);
            Game.Scene.Input.BindMouse(0, EInputType.Released, OnLMBReleased);

            return this;
        }

        #region UI

        public T ShowUI<T>(EPosition position, Action<T> beforeOnShow = null)
            where T : UIBase<T>
        {
            T ui = GetUI<T>();
            UIBase.InvokeSetIgnoreParentGroups(ui, false);
            UIBase.InvokeSetPosition(ui, position);

            beforeOnShow?.Invoke(ui);
            UIBase.InvokeOnShow(ui);

            FocusUI(ui);

            return ui;
        }

        public T ShowModalUI<T>(UIBase parent, EPosition position, Action<T> beforeOnShow = null)
            where T : UIBase<T>
        {
            T ui = GetUI<T>();

            if (parent == null)
            {
                Debug.Assert(_globalModelWindow == null, $"GlobalModalWindow already exists.");

                _globalModelWindow = ui;
                _windowCanvasGroup.interactable = false;
                _hudCanvasGroup.interactable = false;
            }

            UIBase.InvokeSetIgnoreParentGroups(ui, true);
            UIBase.InvokeSetParent(ui, parent);
            UIBase.InvokeSetPosition(ui, position);

            beforeOnShow?.Invoke(ui);
            UIBase.InvokeOnShow(ui);

            FocusUI(ui);

            return ui;
        }

        public void HideUI<T>(UIBase ui)
            where T : UIBase
        {
            Type t = typeof(T);
            Debug.Assert(_cachedUIs.ContainsKey(t));

            if (FocusedUI == ui)
            {
                FocusedUI = null;
            }

            if (_globalModelWindow == ui)
            {
                _globalModelWindow = null;
                _windowCanvasGroup.interactable = true;
                _hudCanvasGroup.interactable = true;
            }

            UIBase.InvokeOnHide(ui);

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

        private T GetUI<T>()
            where T : UIBase
        {
            Type t = typeof(T);
            if (!_cachedUIs.TryGetValue(t, out IObjectPool<UIBase> pool))
            {
                pool = new ObjectPool<UIBase>(_windowContainer, CreateUI<T>, false, 0, 4);
                _cachedUIs.Add(t, pool);
            }

            return (T)pool.Get();
        }

        private UIBase CreateUI<T>()
            where T : UIBase
        {
            Type t = typeof(T);
            if (!_uiPrefabs.TryGetValue(t, out UIBase prefab))
            {
                string key = $"UI/Prefabs/{t.Name}";
                prefab = Resources.Load<T>(key);
                Debug.Assert(prefab != null, $"Not found '{key}'");

                _uiPrefabs.Add(t, prefab);
            }

            UIBase ui = Instantiate(prefab);
            UIBase.InvokeInitialize(ui);
            return ui;
        }

        #endregion

        #region HUD

        public T ShowHUD<T>(Action<T> beforeOnShow = null)
            where T : HUDBase<T>
        {
            Type t = typeof(T);
            if (_cachedHUDs.TryGetValue(t, out HUDBase hudBase))
            {
                Debug.Assert(!hudBase.gameObject.activeSelf, $"{t.Name} already exists.");

                hudBase.gameObject.SetActive(true);
            }
            else
            {
                hudBase = CreateHUD<T>();
                _cachedHUDs.Add(t, hudBase);
            }

            T hud = (T)hudBase;
            beforeOnShow?.Invoke(hud);
            HUDBase.InvokeOnShow(hud);
            hud.RectTransform.SetAsLastSibling();

            return hud;
        }

        public void HideHUD<T>()
            where T : HUDBase
        {
            Type t = typeof(T);
            Debug.Assert(_cachedHUDs.ContainsKey(t) && _cachedHUDs[t].gameObject.activeSelf);

            HUDBase hud = _cachedHUDs[t];
            HUDBase.InvokeOnHide(hud);
            hud.gameObject.SetActive(false);
        }

        public T GetHUD<T>()
            where T : HUDBase
        {
            Type t = typeof(T);
            if (!_cachedHUDs.TryGetValue(t, out HUDBase hud))
            {
                return null;
            }

            return (T)hud;
        }

        private HUDBase CreateHUD<T>()
            where T : HUDBase
        {
            Type t = typeof(T);
            string key = $"UI/Prefabs/{t.Name}";
            HUDBase prefab = Resources.Load<T>(key);
            Debug.Assert(prefab != null, $"Not found '{key}'");

            HUDBase hud = Instantiate(prefab, _hudContainer, false);
            HUDBase.InvokeInitialize(hud);

            return hud;
        }

        #endregion

        private void OnLMBPressed(Vector2 position, EModifier modifier)
        {
            PointerEventData data = new PointerEventData(EventSystem.current);
            data.position = position;

            EventSystem.current.RaycastAll(data, _raycastResults);

            if (_raycastResults.Count > 0)
            {
                RaycastResult result = _raycastResults[0];
                UIBase ui = result.gameObject.GetComponentInParent<UIBase>();

                if (ui != null && ui.Interactable && (_globalModelWindow == null || _globalModelWindow == ui))
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
