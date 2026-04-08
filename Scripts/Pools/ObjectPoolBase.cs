using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Pools
{
    public abstract class ObjectPoolBase<T>
        where T : MonoBehaviour
    {
        private UnityEngine.Pool.IObjectPool<T> _pool;

        protected Transform Parent { get; }
        protected Transform InactiveParent { get; }
        protected bool WorldPositionStays { get; }

        protected ObjectPoolBase(Transform parent, bool parentActive, bool worldPositionStays)
        {
            Debug.Assert(parent != null);

            WorldPositionStays = worldPositionStays;

            Parent = parent;
            GameObject obj = new GameObject("Pool");
            InactiveParent = obj.transform;
            InactiveParent.SetParent(parent, false);
            if (parentActive)
            {
                obj.transform.position = new Vector3(-1000f, -1000f, -1000f);
            }
            else
            {
                obj.SetActive(false);
            }
        }

        protected void Initialize(System.Func<T> onCreateNew, int defaultInstantiateCount, int defaultCapacity, int maxSize = 10000)
        {
            Debug.Assert(defaultInstantiateCount <= defaultCapacity, $"Re-allocating will occur; Set {nameof(defaultCapacity)} bigger than {nameof(defaultInstantiateCount)}");

            _pool = new UnityEngine.Pool.ObjectPool<T>(onCreateNew, null, null, item => Object.Destroy(item.gameObject), false, defaultCapacity, maxSize);

            for (int i = 0; i < defaultInstantiateCount; i++)
            {
                T item = onCreateNew();
                Release(ref item);
            }
        }

        protected T GetInternal()
        {
            return _pool.Get();
        }

        public void Release(ref T item)
        {
            _pool.Release(item);
            item.transform.SetParent(InactiveParent, WorldPositionStays);
            item = null;
        }
    }
}
