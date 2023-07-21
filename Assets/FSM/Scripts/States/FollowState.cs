using Elysia.StateMachines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowState : StateBase<AIController.EState, StateData>
{
    public override AIController.EState State => AIController.EState.Follow;

    public override void OnStart(AIController.EState prevState)
    {
        base.OnStart(prevState);
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        bool bFound = _data.GetTarget(out Vector3 position);
        if (!bFound)
        {
            _stateMachine.Transit(AIController.EState.Return);
        }
        else
        {
            float distance = (position - _data.Transform.position).sqrMagnitude;
            if (distance >= StateData.MAX_FOLLOW_DISTANCE)
            {
                _stateMachine.Transit(AIController.EState.Return);
            }
            else
            {
                _data.Transform.LookAt(position);
                _data.Transform.position = Vector3.MoveTowards(_data.Transform.position, position, StateData.SPEED * deltaTime);
            }
        }
    }

    public override void OnEnd(AIController.EState nextState)
    {
        base.OnEnd(nextState);
    }
}
