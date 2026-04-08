using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    [Serializable]
    public class AssetReferenceT<T> : UnityEngine.AddressableAssets.AssetReferenceT<T>, ISerializationCallbackReceiver
        where T : UnityEngine.Object
    {
        [SerializeField] private string _assetName;
        public string AssetName => _assetName;

        public StringID Key { get; private set; }

        public AssetReferenceT(string guid)
            : base(guid)
        {
        }

        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (!AssetPostProcessor.IsRefreshing)
            {
                return;
            }

            _assetName = AssetPostProcessor.GetAddress(AssetGUID);
#endif
        }

        public void OnAfterDeserialize()
        {
            Key = !string.IsNullOrEmpty(_assetName) ? new StringID(_assetName) : StringID.NULL;
        }

#if UNITY_EDITOR
        public override bool SetEditorAsset(UnityEngine.Object value)
        {
            bool result = base.SetEditorAsset(value);

            _assetName = AssetPostProcessor.GetAddress(AssetGUID);

            return result;
        }
#endif
    }
}
