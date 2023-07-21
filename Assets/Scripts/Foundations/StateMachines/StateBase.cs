using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.StateMachines
{
    public abstract class StateBase<T, TData>
        where T : unmanaged, Enum
    {
        public abstract T State { get; }

        protected IStateMachine<T> _stateMachine;
        protected TData _data;

        public virtual void Initialize(IStateMachine<T> stateMachine, TData data)
        {
            _stateMachine = stateMachine;
            _data = data;
        }

        public virtual void OnStart(T prevState)
        {
        }

        public virtual void OnUpdate(float deltaTime)
        {
        }

        public virtual void OnEnd(T nextState)
        {
        }
    }
}
