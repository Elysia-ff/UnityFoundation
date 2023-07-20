using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.StateMachines
{
    public interface IState<T> where T : unmanaged, Enum
    {
        T State { get; }

        void Initialize(IStateMachine<T> stateMachine);

        void OnStart(T prevState);

        void OnUpdate();

        void OnEnd(T nextState);
    }
}
