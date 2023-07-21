using Elysia;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateData
{
    public Transform Transform { get; }
    public Vector3 ReturnPosition { get; }

    public const float SPEED = 5f;
    public const float FOLLOW_DISTANCE = 50f; // squared
    public const float MAX_FOLLOW_DISTANCE = 100f; // squared

    public StateData(Transform transform)
    {
        Transform = transform;
        ReturnPosition = transform.position;
    }

    public bool GetTarget(out Vector3 outPosition)
    {
        Ray ray = Game.Scene.MainCamera.ScreenPointToRay(Input.mousePosition);

        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        if (!Physics.Raycast(ray, out RaycastHit hitInfo, float.PositiveInfinity, layerMask))
        {
            outPosition = Vector3.zero;
            return false;
        }

        Vector3 pos = hitInfo.point;
        pos.y = Transform.position.y;

        outPosition = pos;
        return true;
    }

    public bool IsTargetInFollowDistance()
    {
        if (GetTarget(out Vector3 position))
        {
            Transform.LookAt(position);

            float distance = (position - Transform.position).sqrMagnitude;
            if (distance <= FOLLOW_DISTANCE)
            {
                return true;
            }
        }

        return false;
    }
}
