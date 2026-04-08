using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elysia
{
    public class ImageRef : UnityEngine.UI.Image
    {
        public StringID ID { get; private set; } = StringID.NULL;

        public override Texture mainTexture
        {
            get
            {
                if (overrideSprite == null)
                {
                    if (material != null && material.mainTexture != null)
                    {
                        return material.mainTexture;
                    }

                    return BLACK_TEXTURE;
                }

                return overrideSprite.texture;
            }
        }

        private VersionHandlePair<Sprite> _pair;
        private int _version;

        protected static Texture2D BLACK_TEXTURE;

        protected override void OnEnable()
        {
            if (BLACK_TEXTURE == null)
            {
                BLACK_TEXTURE = Texture2D.blackTexture;
            }

            base.OnEnable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _pair.Release(ref _version);
        }

        public void SetReference(StringID id, System.Action onCompleted = null)
        {
            if (ID == id)
            {
                onCompleted?.Invoke();
                return;
            }

            sprite = null;
            _pair.Release(ref _version);

            ID = id;
            if (id == StringID.NULL)
            {
                sprite = null;
                onCompleted?.Invoke();
            }
            else if (App.ResourceHolder.TryGetSprite(id, out Sprite s))
            {
                sprite = s;
                onCompleted?.Invoke();
            }
            else
            {
                _pair = new VersionHandlePair<Sprite>(Addressables.LoadAssetAsync<Sprite>((string)id), _version);

                if (_pair.handle.Status == AsyncOperationStatus.Succeeded)
                {
                    sprite = _pair.handle.Result;
                    onCompleted?.Invoke();
                }
                else
                {
                    int version = _version;
                    _pair.handle.Completed += handle =>
                    {
                        if (version == _version && handle.IsDone && handle.Status == AsyncOperationStatus.Succeeded)
                        {
                            sprite = handle.Result;
                            onCompleted?.Invoke();
                        }
                    };
                }
            }
        }

        public void Release()
        {
            _pair.Release(ref _version);

            ID = StringID.NULL;
            sprite = null;
        }
    }
}
