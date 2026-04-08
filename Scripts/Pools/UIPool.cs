using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Pools
{
    public class UIPool<T>
        where T : MonoBehaviour
    {
        private readonly UnityEngine.Pool.IObjectPool<T> _pool;

        public UIPool(System.Func<T> onCreateNew, int defaultInstantiateCount, int defaultCapacity, int maxSize = 10000)
        {
            Debug.Assert(defaultInstantiateCount <= defaultCapacity, $"Re-allocating will occur; Set {nameof(defaultCapacity)} bigger than {nameof(defaultInstantiateCount)}");

            _pool = new UnityEngine.Pool.ObjectPool<T>(onCreateNew, null, null, OnDestroyPoolObject, false, defaultCapacity, maxSize);

            if (defaultInstantiateCount > 0)
            {
                Queue<T> queue = new Queue<T>(defaultInstantiateCount);
                for (int i = 0; i < defaultInstantiateCount; i++)
                {
                    queue.Enqueue(Get());
                }

                while (queue.TryDequeue(out T item))
                {
                    Release(ref item);
                }
            }
        }

        public T Get()
        {
            T item = _pool.Get();

            return item;
        }

        public void Release(ref T item)
        {
            _pool.Release(item);
            item = null;
        }

        private void OnDestroyPoolObject(T item)
        {
            Object.Destroy(item.gameObject);
        }
    }
}
