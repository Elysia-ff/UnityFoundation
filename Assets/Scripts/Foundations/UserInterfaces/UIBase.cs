using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.UI
{
    public abstract class UIBase : MonoBehaviour
    {
        public virtual void Initialize()
        {
        }

        public abstract void Close();
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
