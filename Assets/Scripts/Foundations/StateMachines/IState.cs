using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.StateMachines
{
    public interface IState<T, TData>
        where T : unmanaged, Enum
    {
        T State { get; }

        void Initialize(IStateMachine<T> stateMachine, TData data);

        void OnStart(T prevState);

        void OnUpdate(float deltaTime);

        void OnEnd(T nextState);
    }
}
