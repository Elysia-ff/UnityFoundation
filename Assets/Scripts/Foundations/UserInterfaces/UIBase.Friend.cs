using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.UI
{
    public abstract partial class UIBase
    {
        public static void InvokeInitialize(UIBase ui)
        {
            ui.Initialize();
        }

        public static void InvokeOnFocused(UIBase ui)
        {
            ui.OnFocused();
        }

        public static void InvokeOnInteractableChanged(UIBase ui, bool value)
        {
            ui.OnInteractableChanged(value);
        }

        public static void InvokeOnPointerPressed(UIBase ui)
        {
            ui.OnPointerPressed();
        }
    }
}
