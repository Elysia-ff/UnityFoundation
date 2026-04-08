using System;
using System.Collections;
using System.Collections.Generic;
using Elysia.UI;
using Elysia.VFX;
using Elysia.ResourceManagement;
using UnityEngine;

namespace Elysia
{
    public partial class ResourceHolder
    {
        private Dictionary<StringID, VFXBase> _vfx;
        private Dictionary<StringID, AudioClip> _audioClips;
        private Dictionary<StringID, Sprite> _sprites;
        private Dictionary<Type, UIContainer> _uiContainerPrefabs;
        private Dictionary<StringID, UIElement> _uiElementPrefabs;

        public delegate TAsset AssetProviderDelegate<in TLoad, out TAsset>(TLoad asset);
        public delegate void OnLoadedDelegate<in T>(StringID key, T asset);

        public ResourceHolder EnsureAllocated()
        {
            _vfx ??= new Dictionary<StringID, VFXBase>();
            _audioClips ??= new Dictionary<StringID, AudioClip>();
            _sprites ??= new Dictionary<StringID, Sprite>();
            _uiContainerPrefabs ??= new Dictionary<Type, UIContainer>();
            _uiElementPrefabs ??= new Dictionary<StringID, UIElement>();

            return this;
        }

        public VFXBase GetVFX(StringID key)
        {
            Debug.Assert(_vfx.TryGetValue(key, out VFXBase assertionObj) && assertionObj != null);

            return _vfx[key];
        }

        public AudioClip GetAudioClip(StringID key)
        {
            Debug.Assert(_audioClips.TryGetValue(key, out AudioClip assertionObj) && assertionObj != null);

            return _audioClips[key];
        }

        public Sprite GetSprite(StringID key)
        {
            Debug.Assert(_sprites.TryGetValue(key, out Sprite assertionObj) && assertionObj != null);

            return _sprites[key];
        }

        public bool TryGetSprite(StringID key, out Sprite outSprite)
        {
            return _sprites.TryGetValue(key, out outSprite);
        }

        public T GetUI<T>()
            where T : UIContainer
        {
            Type key = typeof(T);
            Debug.Assert(_uiContainerPrefabs.TryGetValue(key, out UIContainer assertionObj) && assertionObj != null);

            return (T)_uiContainerPrefabs[key];
        }

        public T GetUI<T>(StringID key)
            where T : UIElement
        {
            Debug.Assert(_uiElementPrefabs.TryGetValue(key, out UIElement assertionObj) && assertionObj != null);

            return (T)_uiElementPrefabs[key];
        }

        private void OnObjectLoaded<TLoad, TAsset>(IReadOnlyList<Asset<TLoad>> results, AssetProviderDelegate<TLoad, TAsset> assetProvider, OnLoadedDelegate<TAsset> onLoaded)
        {
            Debug.Assert(results.Count > 0);

            for (int i = 0; i < results.Count; i++)
            {
                Asset<TLoad> asset = results[i];
                StringID key = (StringID)asset.PrimaryKey;

                onLoaded?.Invoke(key, assetProvider(asset.Object));
            }
        }

        private void OnVFXLoaded(IReadOnlyList<Asset<GameObject>> results)
        {
            Debug.Assert(results.Count > 0);

            for (int i = 0; i < results.Count; i++)
            {
                Asset<GameObject> asset = results[i];
                StringID key = (StringID)asset.PrimaryKey;
                Debug.Assert(asset.Object.TryGetComponent(out VFXBase _), $"No {nameof(VFXBase)} component found in {asset.Object.name}", asset.Object);

                VFXBase component = asset.Object.GetComponent<VFXBase>();

                _vfx[key] = component;
            }
        }

        private void OnAudioClipLoaded(IReadOnlyList<Asset<AudioClip>> results)
        {
            Debug.Assert(results.Count > 0);

            for (int i = 0; i < results.Count; i++)
            {
                Asset<AudioClip> asset = results[i];
                StringID key = (StringID)asset.PrimaryKey;

                _audioClips[key] = asset.Object;
            }
        }

        private void OnSpriteLoaded(IReadOnlyList<Asset<Sprite>> results)
        {
            Debug.Assert(results.Count > 0);

            for (int i = 0; i < results.Count; i++)
            {
                Asset<Sprite> asset = results[i];
                StringID key = (StringID)asset.PrimaryKey;

                _sprites[key] = asset.Object;
            }
        }

        private void OnUILoaded(IReadOnlyList<Asset<GameObject>> results)
        {
            Debug.Assert(results.Count > 0);

            for (int i = 0; i < results.Count; i++)
            {
                Asset<GameObject> asset = results[i];
                StringID key = (StringID)asset.PrimaryKey;

                UIElement element = asset.Object.GetComponent<UIElement>();
                Debug.Assert(element != null, $"No {nameof(UIElement)} component found in {asset.Object.name}", asset.Object);

                if (element is UIContainer container)
                {
                    _uiContainerPrefabs[container.GetType()] = container;
                }
                else
                {
                    _uiElementPrefabs[key] = element;
                }
            }
        }
    }
}
