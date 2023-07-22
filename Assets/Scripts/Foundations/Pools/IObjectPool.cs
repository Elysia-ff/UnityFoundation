using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Pools
{
    public interface IObjectPool<T>
    {
        T Get();

        PooledObject<T> GetWrapper();

        void Release(ref T item);
    }
}
