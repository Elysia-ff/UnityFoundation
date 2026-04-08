using Elysia.Pools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.UI
{
    public class MainCanvas : MonoBehaviour, IRootCanvas
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

        public Canvas Canvas { get; private set; }
        public RectTransform Container { get; private set; }
        private CanvasGroup _canvasGroup;

        private readonly Dictionary<Type, UIPool<UIBase>> _cachedUIs = new Dictionary<Type, UIPool<UIBase>>();

        public void Initialize()
        {
            Canvas = transform.GetComponent<Canvas>();
            Container = (RectTransform)transform.Find("Container");
            _canvasGroup = Container.GetComponent<CanvasGroup>();
        }

        public T Show<T>(Action<T> beforeOnShow = null)
            where T : UIBase
        {
            T ui = Get<T>();
            UIBase.InvokeSetIgnoreParentGroups(ui, false);
            UIBase.InvokeSetPosition(ui, EPosition.CenterOfScreen);

            beforeOnShow?.Invoke(ui);

            bool invokeFocusedEvent = Focus_Impl(ui);
            UIBase.InvokeEnable(this, ui);
            UICount++;

            if (invokeFocusedEvent)
            {
                UIBase.InvokeOnFocused(ui);
            }

            return ui;
        }

        public void Focus(UIBase ui)
        {
            if (Focus_Impl(ui))
            {
                UIBase.InvokeOnFocused(FocusedUI);
            }
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
            UIBase ui = Instantiate(prefab, Container, false);
            UIBase.InvokeInitialize(ui);
            return ui;
        }

        [Obsolete("Do not call this method directly. Use UIBase.Close() instead.")]
        public void InvokeHide(UIBase ui)
        {
            Type t = ui.GetType();
            Debug.Assert(_cachedUIs.ContainsKey(t));

            if (FocusedUI == ui)
            {
                FocusedUI = null;
            }

            UIBase.InvokeDisable(ui);
            UICount--;

            _cachedUIs[t].Release(ref ui);
        }

        private bool Focus_Impl(UIBase ui)
        {
            if (FocusedUI == ui)
            {
                return false;
            }

            FocusedUI = ui;

            if (FocusedUI != null)
            {
                FocusedUI.RectTransform.SetAsLastSibling();
                return true;
            }

            return false;
        }
    }
}
