using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.StateMachines
{
    public class StateMachine<T, TData> : IStateMachine<T> where T : unmanaged, Enum
    {
        private StateBase<T, TData> _state;

        public readonly Dictionary<T, StateBase<T, TData>> _states = new Dictionary<T, StateBase<T, TData>>();

        public StateMachine<T, TData> AddState<TState>(TData data) where TState : StateBase<T, TData>, new()
        {
            StateBase<T, TData> newState = new TState();
            newState.Initialize(this, data);

            _states.Add(newState.State, newState);

            return this;
        }

        public void Transit(T stateType)
        {
            T prevState = (-1).ToEnum<int, T>();
            if (_state != null)
            {
                prevState = _state.State;
                _state.OnEnd(stateType);
            }

            _state = _states[stateType];
            _state.OnStart(prevState);
        }

        public void Update(float deltaTime)
        {
            if (_state == null)
            {
                return;
            }

            _state.OnUpdate(deltaTime);
        }
    }
}
