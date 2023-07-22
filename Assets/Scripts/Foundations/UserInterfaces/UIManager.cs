using Elysia.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.UI
{
    public class UIManager : MonoBehaviour
    {
        private RectTransform _windowContainer;

        private readonly Dictionary<Type, UIBase> _uiPrefabs = new Dictionary<Type, UIBase>();
        private readonly Dictionary<Type, IObjectPool<UIBase>> _cachedUIs = new Dictionary<Type, IObjectPool<UIBase>>();

        public UIManager Initialize()
        {
            _windowContainer = (RectTransform)transform.Find("WindowCanvas/Container");

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
            return ui;
        }

        public void HideUI<T>(UIBase ui)
            where T : UIBase
        {
            Type t = typeof(T);
            Debug.Assert(_cachedUIs.ContainsKey(t));

            _cachedUIs[t].Release(ref ui);
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
    }
}
