using Elysia.Inputs;
using Elysia.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Scenes
{
    public class Level0 : MainSceneBase
    {
        private Vector2 _axisInputs = Vector2.zero;

        private IObjectPool<Tester> _testerPool;
        private Queue<PooledObject<Tester>> _testers = new Queue<PooledObject<Tester>>();

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

            Input.BindMouse(0, EInputType.Pressed, OnLMBPressed);
            Input.BindMouse(0, EInputType.Holding, OnLMBHolding);
            Input.BindMouse(0, EInputType.Released, OnLMBReleased);
            Input.BindMouse(0, EInputType.SingleTap, OnLMBSingleTap);
            Input.BindMouse(0, EInputType.DoubleTap, OnLMBDoubleTap);

            Input.BindMouse(1, EInputType.Pressed, OnRMBPressed);

            _testerPool = new ObjectPool<Tester>(transform, () => new GameObject("dd").AddComponent<Tester>(), (t) => Debug.Log("take"), 3, 16, 5);
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

        private void OnLMBPressed(Vector2 position, EModifier modifier)
        {
            Debug.Log($"pressed {position} {modifier}");

            _testers.Enqueue(_testerPool.GetWrapper());
        }

        private void OnLMBHolding(Vector2 position, EModifier modifier)
        {
            Debug.Log($"holding {position} {modifier}");
        }

        private void OnLMBReleased(Vector2 position, EModifier modifier)
        {
            Debug.Log($"released {position} {modifier}");
        }

        private void OnLMBSingleTap(Vector2 position, EModifier modifier)
        {
            Debug.Log($"single {position} {modifier}");
        }

        private void OnLMBDoubleTap(Vector2 position, EModifier modifier)
        {
            Debug.Log($"double {position} {modifier}");
        }

        private void OnRMBPressed(Vector2 position, EModifier modifier)
        {
            Debug.Log($"pressed {position} {modifier}");

            if (_testers.TryDequeue(out PooledObject<Tester> t))
            {
                t.ReturnToPool();
            }
        }
    }
}
