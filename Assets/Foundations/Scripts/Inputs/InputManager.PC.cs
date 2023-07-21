#if UNITY_EDITOR || UNITY_STANDALONE
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Inputs
{
    public partial class InputManager
    {
        private bool GetMouseButtonDown(int button)
        {
            Debug.Assert(button >= 0);

            return Input.GetMouseButtonDown(button);
        }

        private bool GetMouseButton(int button)
        {
            Debug.Assert(button >= 0);

            return Input.GetMouseButton(button);
        }

        public Vector2 GetMousePosition()
        {
            return Input.mousePosition;
        }
    }
}
#endif
