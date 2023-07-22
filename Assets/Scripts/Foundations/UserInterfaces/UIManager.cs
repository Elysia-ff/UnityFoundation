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

        private readonly Dictionary<Type, UIBase> _uiPrefabs = new Dictionary<Type, UIBase>();
        private readonly Dictionary<Type, IObjectPool<UIBase>> _cachedUIs = new Dictionary<Type, IObjectPool<UIBase>>();

        private readonly List<RaycastResult> _raycastResults = new List<RaycastResult>();

        public UIManager Initialize()
        {
            Camera = transform.Find("Camera").GetComponent<Camera>();
            _windowContainer = (RectTransform)transform.Find("WindowCanvas/Container");

            Game.Scene.Input.BindMouse(0, EInputType.Pressed, OnLMBPressed);

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

            T ui = (T)pool.Get();
            FocusUI(ui);

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
            if (FocusedUI == ui)
            {
                return;
            }

            FocusedUI = ui;

            if (FocusedUI != null)
            {
                FocusedUI.RectTransform.SetAsLastSibling();
                FocusedUI.OnFocused();
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
            ui.Initialize();
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
                FocusUI(ui);
            }
        }
    }
}
