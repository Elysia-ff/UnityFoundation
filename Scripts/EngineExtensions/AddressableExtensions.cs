using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public static class AddressableExtensions
    {
        /// <summary>
        /// see <see cref="UnityEngine.AddressableAssets.AssetReference.RuntimeKeyIsValid"/>
        /// </summary>
        public static bool TryParseRuntimeKey(object key, out Guid outGuid)
        {
            string guid = key as string;
            if (string.IsNullOrEmpty(guid))
            {
                outGuid = Guid.Empty;
                return false;
            }

            int subObjectIndex = guid.IndexOf('[');
            if (subObjectIndex != -1) //This means we're dealing with a sub-object and need to convert the runtime key.
            {
                guid = guid.Substring(0, subObjectIndex);
            }

            return Guid.TryParse(guid, out outGuid);
        }

        public static bool IsValidRuntimeKey(object key)
        {
            Debug.Assert(key is string);

            return TryParseRuntimeKey(key, out Guid _);
        }

        public static Guid ParseRuntimeKey(object key)
        {
            TryParseRuntimeKey(key, out Guid guid);
            return guid;
        }
    }
}
