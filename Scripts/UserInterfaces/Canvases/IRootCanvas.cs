using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.UI
{
    public interface IRootCanvas
    {
        [Obsolete("Do not call this method directly. Use UIBase.Close() instead.")]
        void InvokeHide(UIBase ui);
    }
}
