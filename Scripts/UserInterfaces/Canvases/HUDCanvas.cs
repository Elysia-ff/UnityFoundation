using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.UI
{
    public class HUDCanvas : MonoBehaviour
    {
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

        public RectTransform RectTransform { get; private set; }
        public Canvas Canvas { get; private set; }
        public RectTransform Container { get; private set; }
        private CanvasGroup _canvasGroup;

        private readonly Dictionary<Type, HUDBase> _cachedHUDs = new Dictionary<Type, HUDBase>();

        public void Initialize()
        {
            RectTransform = (RectTransform)transform;
            Canvas = transform.GetComponent<Canvas>();
            Container = (RectTransform)transform.Find("Container");
            _canvasGroup = Container.GetComponent<CanvasGroup>();
        }

        public T Show<T>(Action<T> beforeOnShow = null)
            where T : HUDBase<T>
        {
            Type t = typeof(T);
            if (_cachedHUDs.TryGetValue(t, out HUDBase hudBase))
            {
                Debug.Assert(!hudBase.gameObject.activeSelf, $"{t.Name} already exists.");
            }
            else
            {
                hudBase = Create<T>();
                _cachedHUDs.Add(t, hudBase);
            }

            T hud = (T)hudBase;
            hud.RectTransform.SetAsLastSibling();
            beforeOnShow?.Invoke(hud);
            hudBase.gameObject.SetActive(true);
            HUDBase.InvokeOnShow(hud);

            return hud;
        }

        [Obsolete("Do not call this method directly. Use HUDBase.Close() instead.")]
        public void InvokeHide<T>()
            where T : HUDBase
        {
            Type t = typeof(T);
            Debug.Assert(_cachedHUDs.ContainsKey(t) && _cachedHUDs[t].gameObject.activeSelf);

            HUDBase hud = _cachedHUDs[t];
            HUDBase.InvokeOnHide(hud);
            hud.gameObject.SetActive(false);
        }

        public T Get<T>()
            where T : HUDBase
        {
            Type t = typeof(T);
            if (!_cachedHUDs.TryGetValue(t, out HUDBase hud) || hud == null || !hud.gameObject.activeSelf)
            {
                return null;
            }

            return (T)hud;
        }

        public T Toggle<T>(bool show)
            where T : HUDBase<T>
        {
            T hud = Get<T>();
            if (hud == null && show)
            {
                hud = Show<T>();
            }
            else if (hud != null && !show)
            {
                hud.Close();
                hud = null;
            }

            return hud;
        }

        private HUDBase Create<T>()
            where T : HUDBase
        {
            T prefab = App.ResourceHolder.GetUI<T>();
            HUDBase hud = Instantiate(prefab, Container, false);
            HUDBase.InvokeInitialize(hud);

            return hud;
        }
    }
}
