using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Elysia.ResourceManagement
{
    public class LoadProcess<T> : ILoadProcess
    {
        public delegate void OnCompletedDelegate(IReadOnlyList<Asset<T>> results);

        public int Flag { get; }
        public EPhaseType Phase { get; private set; }
        public float PercentComplete => _assetHandle.IsValid() ? _assetHandle.PercentComplete : 0f;

        private readonly OnCompletedDelegate _onCompleted;
        private AsyncOperationHandle<IList<IResourceLocation>> _locationHandle;
        private AsyncOperationHandle<IList<T>> _assetHandle;

        public LoadProcess(int flag, IEnumerable keys, OnCompletedDelegate onCompleted)
        {
            Flag = flag;
            Phase = EPhaseType.LoadingLocation;

            _onCompleted = onCompleted;

            _locationHandle = Addressables.LoadResourceLocationsAsync(keys, Addressables.MergeMode.Intersection, typeof(T));
        }

        public LoadProcess(int flag, string key, OnCompletedDelegate onCompleted)
        {
            Flag = flag;
            Phase = EPhaseType.LoadingLocation;

            _onCompleted = onCompleted;

            _locationHandle = Addressables.LoadResourceLocationsAsync(key, typeof(T));
        }

        public void Unload()
        {
            Addressables.Release(_locationHandle);
            Addressables.Release(_assetHandle);
        }

        public void WaitForLocations()
        {
            Debug.Assert(Phase == EPhaseType.LoadingLocation);

            _locationHandle.WaitForCompletion();

            OnLocationLoaded();
        }

        public IEnumerator WaitForLocationsAsync()
        {
            Debug.Assert(Phase == EPhaseType.LoadingLocation);

            while (!_locationHandle.IsDone)
            {
                yield return null;
            }

            OnLocationLoaded();
        }

        private void OnLocationLoaded()
        {
            Debug.Assert(_locationHandle.Result.Count > 0);
            Phase = EPhaseType.LocationLoaded;
        }

        public void LoadAssets()
        {
            Debug.Assert(Phase == EPhaseType.LocationLoaded);

            Phase = EPhaseType.LoadingAssets;

            IList<IResourceLocation> locations = _locationHandle.Result;
            _assetHandle = Addressables.LoadAssetsAsync<T>(locations, null, true);
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
            IList<IResourceLocation> locations = _locationHandle.Result;
            IList<T> assets = _assetHandle.Result;
            Debug.Assert(_locationHandle.Result.Count == assets.Count);

            List<Asset<T>> results = new List<Asset<T>>(assets.Count);
            for (int i = 0; i < assets.Count; i++)
            {
                results.Add(new Asset<T>(locations[i], assets[i]));
            }

            _onCompleted?.Invoke(results);

            Phase = EPhaseType.Completed;
        }
    }
}
