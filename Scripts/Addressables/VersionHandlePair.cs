using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elysia
{
    public struct VersionHandlePair<T>
    {
        public AsyncOperationHandle<T> handle;
        public readonly int version;

        public VersionHandlePair(AsyncOperationHandle<T> handle, int version)
        {
            this.handle = handle;
            this.version = version;
        }

        public void Release(ref int v)
        {
            if (version == v && handle.IsValid())
            {
                Addressables.Release(handle);
            }

            unchecked
            {
                v++;
            }
        }
    }
}
