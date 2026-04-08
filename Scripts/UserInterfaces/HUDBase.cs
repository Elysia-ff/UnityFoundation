using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.UI
{
    public abstract partial class HUDBase : UIContainer
    {
        public RectTransform RectTransform { get; private set; }

        protected virtual void Initialize()
        {
            RectTransform = (RectTransform)transform;
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        public virtual void Close()
        {
        }
    }

    public abstract class HUDBase<T> : HUDBase
        where T : HUDBase<T>
    {
        public sealed override void Close()
        {
            base.Close();

#pragma warning disable CS0618
            App.Scene.UI.HUD.InvokeHide<T>();
#pragma warning restore CS0618
        }
    }
}
