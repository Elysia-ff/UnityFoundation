using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.StateMachines
{
    public interface IStateMachine<T> where T : unmanaged, Enum
    {
        void Transit(T stateType);

        void Update(float deltaTime);
    }
}
