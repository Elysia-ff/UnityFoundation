using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Elysia.ResourceManagement
{
    public readonly struct Asset<T>
    {
        public string InternalId { get; }
        public string PrimaryKey { get; }
        public T Object { get; }

        public Asset(IResourceLocation location, T obj)
        {
            InternalId = location.InternalId;
            PrimaryKey = location.PrimaryKey;
            Object = obj;
        }

        public Asset(string primaryKey, T obj)
        {
            InternalId = null;
            PrimaryKey = primaryKey;
            Object = obj;
        }
    }
}
