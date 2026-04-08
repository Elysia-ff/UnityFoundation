using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elysia.ResourceManagement
{
    public class GroupLoadProcess<T> : ILoadProcess
    {
        public delegate void OnCompletedDelegate(IReadOnlyList<Asset<T>> results);

        public int Flag { get; }
        public EPhaseType Phase { get; private set; }
        public float PercentComplete => _assetHandle.IsValid() ? _assetHandle.PercentComplete : 0f;

        private readonly OnCompletedDelegate _onCompleted;
        private AsyncOperationHandle<IList<T>> _assetHandle;

        private readonly List<StringID> _keys;

        public GroupLoadProcess(int flag, List<StringID> keys, OnCompletedDelegate onCompleted)
        {
            Flag = flag;
            Phase = EPhaseType.LoadingLocation;

            _keys = keys;
            _onCompleted = onCompleted;
        }

        public void Unload()
        {
            Addressables.Release(_assetHandle);
        }

        public void WaitForLocations()
        {
            OnLocationLoaded();
        }

        public IEnumerator WaitForLocationsAsync()
        {
            OnLocationLoaded();
            yield break;
        }

        private void OnLocationLoaded()
        {
            Phase = EPhaseType.LocationLoaded;
        }

        public void LoadAssets()
        {
            Debug.Assert(Phase == EPhaseType.LocationLoaded);

            Phase = EPhaseType.LoadingAssets;

            _assetHandle = Addressables.LoadAssetsAsync<T>(_keys, null, Addressables.MergeMode.Union);
        }

        public void WaitForAssets()
        {
            Debug.Assert(Phase == EPhaseType.LoadingAssets);

            _assetHandle.WaitForCompletion();

            OnAssetLoaded();
        }

        public bool WaitForAssetsAsync()
        {
            Debug.Assert(Phase == EPhaseType.LoadingAssets);

            if (!_assetHandle.IsDone)
            {
                return false;
            }

            OnAssetLoaded();
            return true;
        }

        private void OnAssetLoaded()
        {
            IList<T> assets = _assetHandle.Result;
            Debug.Assert(_keys.Count == assets.Count);

            List<Asset<T>> results = new List<Asset<T>>(assets.Count);
            for (int i = 0; i < assets.Count; i++)
            {
                results.Add(new Asset<T>((string)_keys[i], assets[i]));
            }

            _onCompleted?.Invoke(results);

            Phase = EPhaseType.Completed;
        }
    }
}
