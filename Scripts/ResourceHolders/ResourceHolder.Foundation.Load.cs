using System.Collections;
using System.Collections.Generic;
using Elysia.ResourceManagement;
using UnityEngine;

namespace Elysia
{
    public partial class ResourceHolder
    {
        private readonly List<ILoadProcess> _loadedAssets = new List<ILoadProcess>();
        private readonly List<ILoadProcess> _assetsToBeLoaded = new List<ILoadProcess>();

        private void LoadAssets()
        {
            for (int i = 0; i < _assetsToBeLoaded.Count; i++)
            {
                _assetsToBeLoaded[i].WaitForLocations();
            }

            for (int i = 0; i < _assetsToBeLoaded.Count; i++)
            {
                _assetsToBeLoaded[i].LoadAssets();
            }

            for (int i = 0; i < _assetsToBeLoaded.Count; i++)
            {
                _assetsToBeLoaded[i].WaitForAssets();
            }

            _loadedAssets.AddRange(_assetsToBeLoaded);
            _assetsToBeLoaded.Clear();
        }

        private IEnumerator LoadAssetsAsync(ILoadingReceiver.Data loadingReceiver)
        {
            loadingReceiver?.SetProgress(0f);

            for (int i = 0; i < _assetsToBeLoaded.Count; i++)
            {
                yield return _assetsToBeLoaded[i].WaitForLocationsAsync();
            }

            for (int i = 0; i < _assetsToBeLoaded.Count; i++)
            {
                ILoadProcess p = _assetsToBeLoaded[i];

                p.LoadAssets();
                while (!p.WaitForAssetsAsync())
                {
                    yield return null;
                    loadingReceiver?.SetProgress(GetPercentComplete());
                }
            }

            loadingReceiver?.SetProgress(1f);

            _loadedAssets.AddRange(_assetsToBeLoaded);
            _assetsToBeLoaded.Clear();
        }

        private float GetPercentComplete()
        {
            float percent = 0f;
            for (int i = 0; i < _assetsToBeLoaded.Count; i++)
            {
                percent += _assetsToBeLoaded[i].PercentComplete;
            }

            return percent / _assetsToBeLoaded.Count;
        }

        private void Unload()
        {
            for (int i = 0; i < _loadedAssets.Count; i++)
            {
                _loadedAssets[i].Unload();
            }

            _loadedAssets.Clear();
        }

        private void Unload(int flag)
        {
            for (int i = _loadedAssets.Count - 1; i >= 0; i--)
            {
                ILoadProcess p = _loadedAssets[i];

                if ((p.Flag & flag) == p.Flag)
                {
                    p.Unload();
                }

                _loadedAssets.RemoveAt(i);
            }
        }

        private void LoadObject<T>(int flag, IEnumerable keys, OnLoadedDelegate<T> onLoaded)
        {
            LoadProcess<T> p = new LoadProcess<T>(flag, keys, results =>
            {
                OnObjectLoaded(results, asset => asset, onLoaded);
            });

            _assetsToBeLoaded.Add(p);
        }

        private void LoadObject<T>(int flag, string key, OnLoadedDelegate<T> onLoaded)
        {
            LoadProcess<T> p = new LoadProcess<T>(flag, key, results =>
            {
                OnObjectLoaded(results, asset => asset, onLoaded);
            });

            _assetsToBeLoaded.Add(p);
        }

        private void LoadObject<T>(int flag, List<StringID> keys, OnLoadedDelegate<T> onLoaded)
        {
            GroupLoadProcess<T> p = new GroupLoadProcess<T>(flag, keys, results =>
            {
                OnObjectLoaded(results, asset => asset, onLoaded);
            });

            _assetsToBeLoaded.Add(p);
        }

        private void LoadObject<TLoad, TAsset>(int flag, IEnumerable keys, AssetProviderDelegate<TLoad, TAsset> assetProvider, OnLoadedDelegate<TAsset> onLoaded)
        {
            LoadProcess<TLoad> p = new LoadProcess<TLoad>(flag, keys, results =>
            {
                OnObjectLoaded(results, assetProvider, onLoaded);
            });

            _assetsToBeLoaded.Add(p);
        }

        private void LoadObject<TLoad, TAsset>(int flag, string key, AssetProviderDelegate<TLoad, TAsset> assetProvider, OnLoadedDelegate<TAsset> onLoaded)
        {
            LoadProcess<TLoad> p = new LoadProcess<TLoad>(flag, key, results =>
            {
                OnObjectLoaded(results, assetProvider, onLoaded);
            });

            _assetsToBeLoaded.Add(p);
        }

        private void LoadObject<TLoad, TAsset>(int flag, List<StringID> keys, AssetProviderDelegate<TLoad, TAsset> assetProvider, OnLoadedDelegate<TAsset> onLoaded)
        {
            GroupLoadProcess<TLoad> p = new GroupLoadProcess<TLoad>(flag, keys, results =>
            {
                OnObjectLoaded(results, assetProvider, onLoaded);
            });

            _assetsToBeLoaded.Add(p);
        }

        private void LoadVFX(int flag, IEnumerable keys)
        {
            LoadProcess<GameObject> p = new LoadProcess<GameObject>(flag, keys, OnVFXLoaded);

            _assetsToBeLoaded.Add(p);
        }

        private void LoadVFX(int flag, string key)
        {
            LoadProcess<GameObject> p = new LoadProcess<GameObject>(flag, key, OnVFXLoaded);

            _assetsToBeLoaded.Add(p);
        }

        private void LoadVFX(int flag, List<StringID> keys)
        {
            GroupLoadProcess<GameObject> p = new GroupLoadProcess<GameObject>(flag, keys, OnVFXLoaded);

            _assetsToBeLoaded.Add(p);
        }

        private void LoadAudioClip(int flag, IEnumerable keys)
        {
            LoadProcess<AudioClip> p = new LoadProcess<AudioClip>(flag, keys, OnAudioClipLoaded);

            _assetsToBeLoaded.Add(p);
        }

        private void LoadAudioClip(int flag, string key)
        {
            LoadProcess<AudioClip> p = new LoadProcess<AudioClip>(flag, key, OnAudioClipLoaded);

            _assetsToBeLoaded.Add(p);
        }

        private void LoadAudioClip(int flag, List<StringID> keys)
        {
            GroupLoadProcess<AudioClip> p = new GroupLoadProcess<AudioClip>(flag, keys, OnAudioClipLoaded);

            _assetsToBeLoaded.Add(p);
        }

        private void LoadSprite(int flag, IEnumerable keys)
        {
            LoadProcess<Sprite> p = new LoadProcess<Sprite>(flag, keys, OnSpriteLoaded);

            _assetsToBeLoaded.Add(p);
        }

        private void LoadSprite(int flag, string key)
        {
            LoadProcess<Sprite> p = new LoadProcess<Sprite>(flag, key, OnSpriteLoaded);

            _assetsToBeLoaded.Add(p);
        }

        private void LoadSprite(int flag, List<StringID> keys)
        {
            GroupLoadProcess<Sprite> p = new GroupLoadProcess<Sprite>(flag, keys, OnSpriteLoaded);

            _assetsToBeLoaded.Add(p);
        }

        private void LoadUI(int flag, IEnumerable keys)
        {
            LoadProcess<GameObject> p = new LoadProcess<GameObject>(flag, keys, OnUILoaded);

            _assetsToBeLoaded.Add(p);
        }

        private void LoadUI(int flag, string key)
        {
            LoadProcess<GameObject> p = new LoadProcess<GameObject>(flag, key, OnUILoaded);

            _assetsToBeLoaded.Add(p);
        }
    }
}
