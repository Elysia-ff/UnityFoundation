using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Pool
{
    public interface IObjectPool<T>
    {
        T Get();

        PooledObject<T> GetWrapper();

        void Release(ref T item);
    }
}
