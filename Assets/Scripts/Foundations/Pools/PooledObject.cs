using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Pool
{
    public struct PooledObject<T> : IDisposable
    {
        private T _item;
        public readonly T Item => _item;
        private readonly IObjectPool<T> _pool;

        public PooledObject(T item, IObjectPool<T> pool)
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
