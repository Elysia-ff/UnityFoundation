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
        }

        private void OnVerticalInput(float value)
        {
            _axisInputs.y = value;
        }

        private void OnRPressed(EModifier modifier)
        {
            Debug.Log($"pressed {modifier}");
        }

        private void OnRHolding(EModifier modifier)
        {
            Debug.Log($"holding {modifier}");
        }

        private void OnRReleased(EModifier modifier)
        {
            Debug.Log($"released {modifier}");
        }

        private void OnRSingleTap(EModifier modifier)
        {
            Debug.Log($"single {modifier}");
        }

        private void OnRDoubleTap(EModifier modifier)
        {
            Debug.Log($"double {modifier}");
        }
    }
}
