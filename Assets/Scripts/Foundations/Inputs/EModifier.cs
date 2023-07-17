using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Inputs
{
    [Flags]
    public enum EModifier
    {
        None = 0,
        Ctrl    = 1 << 0,
        Shift   = 1 << 1,
        Alt     = 1 << 2,

        CtrlShift = Ctrl | Shift,
        CtrlAlt = Ctrl | Alt,
        ShiftAlt = Shift | Alt,
        CtrlShiftAlt = Ctrl | Shift | Alt,
    }
}
