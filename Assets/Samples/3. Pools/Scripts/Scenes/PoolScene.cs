using Elysia.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Scenes
{
    public class PoolScene : MainSceneBase
    {
        private IObjectPool<SomeObject> _pool;
        private Queue<PooledObject<SomeObject>> _pooledObjs = new Queue<PooledObject<SomeObject>>();

        public override void Initialize(int handle)
        {
            base.Initialize(handle);

            _pool = new ObjectPool<SomeObject>(transform,
                () => new GameObject(nameof(SomeObject)).AddComponent<SomeObject>(),
                (t) => Debug.Log("take"),
                worldPositionStays: false,
                defaultInstantiateCount: 3,
                defaultCapacity: 16,
                maxSize: 5);
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(20, 20, Screen.width - 40, 60), "Take"))
            {
                _pooledObjs.Enqueue(_pool.GetWrapper());
            }

            if (GUI.Button(new Rect(20, 100, Screen.width - 40, 60), "Return"))
            {
                if (_pooledObjs.TryDequeue(out PooledObject<SomeObject> t))
                {
                    t.ReturnToPool();
                }
            }
        }
    }
}
