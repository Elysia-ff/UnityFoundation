using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.UI
{
    public abstract partial class HUDBase : MonoBehaviour
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

            Game.Scene.UI.HideHUD<T>();
        }
    }
}
