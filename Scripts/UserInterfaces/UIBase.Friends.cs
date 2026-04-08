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

        public static void InvokeEnable(IRootCanvas rootCanvas, UIBase ui)
        {
            ui.SetRootCanvas(rootCanvas);
            ui.Enable();
        }

        public static void InvokeDisable(UIBase ui)
        {
            ui.SetRootCanvas(null);
            ui.Disable();
        }

        public static void InvokeOnFocused(UIBase ui)
        {
            ui.OnFocused();
        }

        public static void InvokeOnPointerPressed(UIBase ui)
        {
            ui.OnPointerPressed();
        }

        public static void InvokeSetParent(UIBase ui, UIBase parent)
        {
            ui.SetParent(parent);
        }

        public static void InvokeSetPosition(UIBase ui, EPosition position)
        {
            ui.SetPosition(position);
        }

        public static void InvokeSetIgnoreParentGroups(UIBase ui, bool value)
        {
            ui.SetIgnoreParentGroups(value);
        }
    }
}
