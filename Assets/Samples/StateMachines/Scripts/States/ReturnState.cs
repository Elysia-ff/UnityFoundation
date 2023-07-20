using Elysia.StateMachines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnState : StateBase<AIController.EState, StateData>
{
    public override AIController.EState State => AIController.EState.Return;

    public override void OnStart(AIController.EState prevState)
    {
        base.OnStart(prevState);
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);

        if (Data.IsTargetInFollowDistance())
        {
            StateMachine.Transit(AIController.EState.Follow);
        }
        else
        {
            Vector3 destination = Data.ReturnPosition;
            Data.Transform.LookAt(destination);
            Data.Transform.position = Vector3.MoveTowards(Data.Transform.position, destination, StateData.SPEED * deltaTime);

            if (Data.Transform.position == destination)
            {
                StateMachine.Transit(AIController.EState.Idle);
            }
        }
    }

    public override void OnEnd(AIController.EState nextState)
    {
        base.OnEnd(nextState);
    }
}
