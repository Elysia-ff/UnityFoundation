using Elysia.StateMachines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase : IState<AIController.EState, StateData>
{
    public abstract AIController.EState State { get; }

    protected IStateMachine<AIController.EState> _stateMachine;
    protected StateData _data;

    public virtual void Initialize(IStateMachine<AIController.EState> stateMachine, StateData data)
    {
        _stateMachine = stateMachine;
        _data = data;
    }

    public virtual void OnStart(AIController.EState prevState)
    {
        Debug.Log($"Start {State} ({prevState})");
    }

    public virtual void OnUpdate(float deltaTime)
    {
    }

    public virtual void OnEnd(AIController.EState nextState)
    {
        Debug.Log($"End {State} ({nextState})");
    }
}
