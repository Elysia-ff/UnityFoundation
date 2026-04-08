using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.VFX
{
    public class VFXManager : MonoBehaviour
    {
        private readonly Dictionary<StringID, Stack<VFXBase>> _vfxPool = new Dictionary<StringID, Stack<VFXBase>>();
        private Transform _poolParent;

        private bool _isRunning;
        private readonly List<VFXBase> _disabledVFXes = new List<VFXBase>(8);

        public void Initialize()
        {
            Transform parent = transform;

            _poolParent = new GameObject("VFXPool").transform;
            _poolParent.SetParent(parent, false);

            _isRunning = true;
            enabled = false;
        }

        public VFXRef CreateAt(StringID key, Vector3 position)
        {
            return CreateAt(key, null, position, Quaternion.identity, Vector3.one);
        }

        public VFXRef CreateAt(StringID key, Vector3 position, Quaternion rotation)
        {
            return CreateAt(key, null, position, rotation, Vector3.one);
        }

        public VFXRef CreateAt(StringID key, Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            return CreateAt(key, null, position, rotation, localScale);
        }

        public VFXRef CreateAt(StringID key, Transform parent, Vector3 position)
        {
            return CreateAt(key, parent, position, Quaternion.identity, Vector3.one);
        }

        public VFXRef CreateAt(StringID key, Transform parent, Vector3 position, Quaternion rotation)
        {
            return CreateAt(key, parent, position, rotation, Vector3.one);
        }

        public VFXRef CreateAt(StringID key, Transform parent, Vector3 position, Quaternion rotation, Vector3 localScale)
        {
            VFXBase vfx = CreateVFX(key);

            Transform vfxTransform = vfx.transform;
            vfxTransform.SetParent(parent, false);
            vfxTransform.SetPositionAndRotation(position, rotation);
            vfxTransform.localScale = localScale;

            vfx.gameObject.SetActive(true);
            vfx.OnStart();

            return new VFXRef(vfx);
        }

        private VFXBase CreateVFX(StringID key)
        {
            if (!_vfxPool.TryGetValue(key, out Stack<VFXBase> pool))
            {
                pool = new Stack<VFXBase>();
                _vfxPool.Add(key, pool);
            }

            if (!pool.TryPop(out VFXBase vfx))
            {
                vfx = Instantiate(App.ResourceHolder.GetVFX(key));
                vfx.Initialize(key);
            }

            return vfx;
        }

        public void OnVFXStopped(VFXBase vfx)
        {
            if (!_isRunning)
            {
                Destroy(vfx.gameObject);
                return;
            }

            vfx.gameObject.SetActive(false);
            vfx.transform.SetParent(_poolParent, false);
            Debug.Assert(_vfxPool.ContainsKey(vfx.Key));
            _vfxPool[vfx.Key].Push(vfx);
        }

        public void EnqueueDisabledVFX(VFXBase vfx)
        {
            if (!_isRunning)
            {
                return;
            }

            _disabledVFXes.Add(vfx);
            enabled = true;
        }

        private void Update()
        {
            for (int i = 0; i < _disabledVFXes.Count; i++)
            {
                _disabledVFXes[i].Stop();
            }

            _disabledVFXes.Clear();
            enabled = false;
        }

        private void OnDestroy()
        {
            _disabledVFXes.Clear();

            _isRunning = false;
        }
    }
}
