using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Pools
{
    public struct PooledObject<T> : IDisposable
        where T : MonoBehaviour
    {
        private T _item;
        public readonly T Item => _item;
        private readonly ObjectPoolBase<T> _pool;

        public PooledObject(T item, ObjectPoolBase<T> pool)
        {
            _item = item;
            _pool = pool;
        }

        public void ReturnToPool()
        {
            Debug.Assert(_item != null);

            _pool.Release(ref _item);
        }

        public void Dispose()
        {
            ReturnToPool();
        }
    }
}
