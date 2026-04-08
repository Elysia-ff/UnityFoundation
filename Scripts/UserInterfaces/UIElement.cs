using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.UI
{
    [DisallowMultipleComponent]
    public abstract class UIElement : MonoBehaviour
    {
        // private static readonly List<CanvasGroup> _canvasGroups = new List<CanvasGroup>(1);
        //
        // private static bool IsVisible(Transform t)
        // {
        //     Debug.Assert(t != null);
        //
        //     while (t != null)
        //     {
        //         t.GetComponents(_canvasGroups);
        //         Debug.Assert(_canvasGroups.Count <= 1);
        //
        //         if (_canvasGroups.Count == 1 && _canvasGroups[0].alpha == 0f)
        //         {
        //             return false;
        //         }
        //
        //         t = t.parent;
        //     }
        //
        //     return true;
        // }
        //
        // private void OnTransformParentChanged()
        // {
        //     enabled = IsVisible(transform);
        // }
        //
        // private void OnCanvasGroupChanged()
        // {
        //     enabled = IsVisible(transform);
        // }
    }
}
