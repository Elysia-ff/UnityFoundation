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

        bool bFound = Data.GetTarget(out Vector3 position);
        if (!bFound)
        {
            StateMachine.Transit(AIController.EState.Return);
        }
        else
        {
            float distance = (position - Data.Transform.position).sqrMagnitude;
            if (distance >= StateData.MAX_FOLLOW_DISTANCE)
            {
                StateMachine.Transit(AIController.EState.Return);
            }
            else
            {
                Data.Transform.LookAt(position);
                Data.Transform.position = Vector3.MoveTowards(Data.Transform.position, position, StateData.SPEED * deltaTime);
            }
        }
    }

    public override void OnEnd(AIController.EState nextState)
    {
        base.OnEnd(nextState);
    }
}
