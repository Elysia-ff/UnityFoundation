using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Elysia
{
    public partial class AudioClipReference
    {
        public interface IData
        {
            string Key { get; }
            float VolumeOverride { get; }
        }

        [Serializable]
        private struct Data : IData
        {
            [SerializeField] private AssetReferenceT<AudioClip> _assetRef;

            [SerializeField][ReadOnly] private string _key;
            public string Key => _key;

            [SerializeField][Range(0f, 1f)] private float _volumeOverride;
            public float VolumeOverride => _volumeOverride;

            public void RefreshKey()
            {
                if (!_assetRef.RuntimeKeyIsValid())
                {
                    _key = string.Empty;
                    return;
                }

                IList<IResourceLocation> locations = Addressables.LoadResourceLocationsAsync(_assetRef.AssetGUID, typeof(AudioClip)).WaitForCompletion();
                Debug.Assert(locations.Count == 1);

                _key = locations[0].PrimaryKey;
            }
        }
    }
}
