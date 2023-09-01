using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Animations
{
    public enum EStopMethod
    {
        None,
        OnOtherAnimationStarted,
        OnAnimationContainsGivenStringStarted,
        OnAnimationDoesNotContainsGivenStringStarted
    }
}
