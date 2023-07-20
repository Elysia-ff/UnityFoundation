using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.StateMachines
{
    public class StateMachine<T> : IStateMachine<T> where T : unmanaged, Enum
    {
        private IState<T> _state;

        public readonly Dictionary<T, IState<T>> _states = new Dictionary<T, IState<T>>();

        public StateMachine<T> AddState<TState>(T stateType) where TState : IState<T>, new()
        {
            _states.Add(stateType, new TState());

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
    }
}
