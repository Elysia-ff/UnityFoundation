using Elysia.Inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Scenes
{
    public class Level0 : MainSceneBase
    {
        private Vector2 _axisInputs = Vector2.zero;

        public override void Initialize(int handle)
        {
            base.Initialize(handle);

            // DEBUGCODE
            Input.BindAxis("Horizontal", OnHorizontalInput);
            Input.BindAxis("Vertical", OnVerticalInput);
            Input.BindKey(KeyCode.R, EInputType.Pressed, OnRPressed);
            Input.BindKey(KeyCode.R, EInputType.Holding, OnRHolding);
            Input.BindKey(KeyCode.R, EInputType.Released, OnRReleased);
            Input.BindKey(KeyCode.R, EInputType.SingleTap, OnRSingleTap);
            Input.BindKey(KeyCode.R, EInputType.DoubleTap, OnRDoubleTap);
        }

        private void OnHorizontalInput(float value)
        {
            _axisInputs.x = value;
            Debug.Log($"x : {value}");

            if (value >= 1f)
            {
                Input.UnbindAxis("Horizontal", OnHorizontalInput);
            }
        }

        private void OnVerticalInput(float value)
        {
            _axisInputs.y = value;
            Debug.Log($"y : {value}");

            if (value >= 1f)
            {
                Input.UnbindAxis("Vertical", OnVerticalInput);
            }
        }

        private void OnRPressed()
        {
            Debug.Log("pressed");

            Input.UnbindKey(KeyCode.R, EInputType.Pressed, OnRPressed);
        }

        private void OnRHolding()
        {
            Debug.Log("holding");

            Input.UnbindKey(KeyCode.R, EInputType.Holding, OnRHolding);
        }

        private void OnRReleased()
        {
            Debug.Log("released");

            Input.UnbindKey(KeyCode.R, EInputType.Released, OnRReleased);
        }

        private void OnRSingleTap()
        {
            Debug.Log("single");

            Input.UnbindKey(KeyCode.R, EInputType.SingleTap, OnRSingleTap);
        }

        private void OnRDoubleTap()
        {
            Debug.Log("double");

            Input.UnbindKey(KeyCode.R, EInputType.DoubleTap, OnRDoubleTap);
        }
    }
}
