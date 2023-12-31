using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Pools
{
    public class ObjectPool<T> : IObjectPool<T>
        where T : MonoBehaviour
    {
        public delegate T OnCreateNewEvent();

        private readonly UnityEngine.Pool.IObjectPool<T> _pool;

        private readonly Transform _parent;
        private readonly Transform _inactiveParent;

        private readonly OnCreateNewEvent _onCreateNew;
        private readonly bool _worldPositionStays;

        public ObjectPool(Transform parent, OnCreateNewEvent onCreateNew, bool worldPositionStays, int defaultInstantiateCount, int defaultCapacity, int maxSize = 10000)
        {
            Debug.Assert(parent != null);
            Debug.Assert(defaultInstantiateCount <= defaultCapacity, $"Re-allocating will occur; Set {nameof(defaultCapacity)} bigger than {nameof(defaultInstantiateCount)}");

            _onCreateNew = onCreateNew;
            _worldPositionStays = worldPositionStays;

            _pool = new UnityEngine.Pool.ObjectPool<T>(OnCreateItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, false, defaultCapacity, maxSize);

            _parent = parent;
            GameObject obj = new GameObject("Pool");
            _inactiveParent = obj.transform;
            _inactiveParent.SetParent(parent, false);
            obj.SetActive(false);

            if (defaultInstantiateCount > 0)
            {
                Queue<T> queue = new Queue<T>(defaultInstantiateCount);
                for (int i = 0; i < defaultInstantiateCount; i++)
                {
                    queue.Enqueue(_pool.Get());
                }

                while (queue.TryDequeue(out T item))
                {
                    _pool.Release(item);
                }
            }
        }

        public T Get()
        {
            return _pool.Get();
        }

        public PooledObject<T> GetWrapper()
        {
            return new PooledObject<T>(Get(), this);
        }

        public void Release(ref T item)
        {
            _pool.Release(item);
            item = null;
        }

        private T OnCreateItem()
        {
            T item = _onCreateNew();

            return item;
        }

        private void OnTakeFromPool(T item)
        {
            item.transform.SetParent(_parent, _worldPositionStays);
        }

        private void OnReturnedToPool(T item)
        {
            item.transform.SetParent(_inactiveParent, _worldPositionStays);
        }

        private void OnDestroyPoolObject(T item)
        {
            UnityEngine.Object.Destroy(item.gameObject);
        }
    }
}
