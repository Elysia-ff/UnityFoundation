#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Inputs
{
    public partial class InputManager
    {
        private int lastDownedTouchID = -1;

        private bool GetMouseButtonDown(int button)
        {
            Debug.Assert(button >= 0);
            if (Input.touchCount <= button)
            {
                return false;
            }

            Touch touch = Input.GetTouch(button);
            return touch.phase == TouchPhase.Began;
        }

        private bool GetMouseButton(int button)
        {
            Debug.Assert(button >= 0);
            if (Input.touchCount <= button)
            {
                lastDownedTouchID = -1;
                return false;
            }

            Touch touch = Input.GetTouch(button);
            bool result = touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary;
            if (!result)
            {
                lastDownedTouchID = -1;
            }

            return result;
        }

        public Vector2 GetMousePosition()
        {
            return lastDownedTouchID >= 0 ? Input.GetTouch(lastDownedTouchID).position : Input.mousePosition;
        }
    }
}
#endif
