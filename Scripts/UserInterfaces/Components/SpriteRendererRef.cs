using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elysia
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRendererRef : MonoBehaviour
    {
        [SerializeField][ReadOnly] private SpriteRenderer _component;
        public SpriteRenderer Component => _component;

        public StringID ID { get; private set; } = StringID.NULL;

        private VersionHandlePair<Sprite> _pair;
        private int _version;

#if UNITY_EDITOR
        private void Reset()
        {
            _component = GetComponent<SpriteRenderer>();
        }

        private void OnValidate()
        {
            _component = GetComponent<SpriteRenderer>();
        }
#endif

        private void OnDestroy()
        {
            _pair.Release(ref _version);
        }

        public void SetReference(StringID id, System.Action onCompleted = null)
        {
            if (ID == id)
            {
                onCompleted?.Invoke();
                return;
            }

            _component.sprite = null;
            _pair.Release(ref _version);

            ID = id;
            if (id == StringID.NULL)
            {
                _component.sprite = null;
                onCompleted?.Invoke();
            }
            else if (App.ResourceHolder.TryGetSprite(id, out Sprite s))
            {
                _component.sprite = s;
                onCompleted?.Invoke();
            }
            else
            {
                _pair = new VersionHandlePair<Sprite>(Addressables.LoadAssetAsync<Sprite>((string)id), _version);

                if (_pair.handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _component.sprite = _pair.handle.Result;
                    onCompleted?.Invoke();
                }
                else
                {
                    int version = _version;
                    _pair.handle.Completed += handle =>
                    {
                        if (version == _version && handle.IsDone && handle.Status == AsyncOperationStatus.Succeeded && _component != null)
                        {
                            _component.sprite = handle.Result;
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
            _component.sprite = null;
        }
    }
}
