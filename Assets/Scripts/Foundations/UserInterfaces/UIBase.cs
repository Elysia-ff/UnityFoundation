using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.UI
{
    public abstract class UIBase : MonoBehaviour
    {
        public RectTransform RectTransform { get; private set; }

        public virtual void Initialize()
        {
            RectTransform = (RectTransform)transform;
        }

        public abstract void Close();

        public virtual void OnFocused()
        {
        }
    }

    public class UIBase<T> : UIBase
        where T : UIBase<T>
    {
        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Close()
        {
            Game.Scene.UI.HideUI<T>(this);
        }
    }
}
