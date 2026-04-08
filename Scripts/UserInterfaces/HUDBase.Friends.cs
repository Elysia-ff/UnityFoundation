using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.UI
{
    public abstract partial class HUDBase
    {
        public static void InvokeInitialize(HUDBase hud)
        {
            hud.Initialize();
        }

        public static void InvokeOnShow(HUDBase hud)
        {
            hud.OnShow();
        }

        public static void InvokeOnHide(HUDBase hud)
        {
            hud.OnHide();
        }
    }
}
