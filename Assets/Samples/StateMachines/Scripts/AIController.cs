using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elysia.StateMachines;

public class AIController : MonoBehaviour
{
    public enum EState
    {
        Idle,
        Follow,
        Return,
    }

    private IStateMachine<EState> _stateMachine;
    private StateData _data;

    private void Awake()
    {
        _data = new StateData(transform);

        _stateMachine = new StateMachine<EState, StateData>()
            .AddState<IdleState>(_data)
            .AddState<FollowState>(_data)
            .AddState<ReturnState>(_data);

        _stateMachine.Transit(EState.Idle);
    }

    private void Update()
    {
        _stateMachine.Update(Time.deltaTime);
    }
}
