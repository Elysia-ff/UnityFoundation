using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnState : StateBase
{
    public override AIController.EState State => AIController.EState.Return;

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
        else
        {
            Vector3 destination = _data.ReturnPosition;
            _data.Transform.LookAt(destination);
            _data.Transform.position = Vector3.MoveTowards(_data.Transform.position, destination, StateData.SPEED * deltaTime);

            if (_data.Transform.position == destination)
            {
                _stateMachine.Transit(AIController.EState.Idle);
            }
        }
    }

    public override void OnEnd(AIController.EState nextState)
    {
        base.OnEnd(nextState);
    }
}
