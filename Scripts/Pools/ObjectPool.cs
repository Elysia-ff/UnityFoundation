using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Pools
{
    public sealed class ObjectPool<T> : ObjectPoolBase<T>
        where T : MonoBehaviour
    {
        public ObjectPool(Transform parent, System.Func<T> onCreateNew, bool parentActive, bool worldPositionStays, int defaultInstantiateCount, int defaultCapacity, int maxSize = 10000)
            : base(parent, parentActive, worldPositionStays)
        {
            Initialize(onCreateNew, defaultInstantiateCount, defaultCapacity, maxSize);
        }

        public T Get()
        {
            return Get(Parent);
        }

        public T Get(Transform parent)
        {
            T item = GetInternal();
            item.transform.SetParent(parent, WorldPositionStays);

            return item;
        }

        public PooledObject<T> GetWrapper()
        {
            return new PooledObject<T>(Get(), this);
        }
    }
}
