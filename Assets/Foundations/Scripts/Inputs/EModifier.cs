using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Inputs
{
    [Flags]
    public enum EModifier
    {
        Ctrl    = 1 << 0,
        Shift   = 1 << 1,
        Alt     = 1 << 2,
    }
}
