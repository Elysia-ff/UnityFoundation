using Elysia.StateMachines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : StateBase<AIController.EState, StateData>
{
    public override AIController.EState State => AIController.EState.Idle;

    public override void OnStart(AIController.EState prevState)
    {
        base.OnStart(prevState);
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (_data.IsTargetInFollowDistance())
        {
            _stateMachine.Transit(AIController.EState.Follow);
        }
    }

    public override void OnEnd(AIController.EState nextState)
    {
        base.OnEnd(nextState);
    }
}
