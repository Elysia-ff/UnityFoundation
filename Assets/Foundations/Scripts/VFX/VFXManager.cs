using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.VFX
{
    public interface IPooledVFX
    {
        string Key { get; }
        bool IsPlaying { get; }
        void ReturnToPool();
    }

    public class VFXManager : MonoBehaviour
    {
        private readonly Dictionary<string, Stack<PooledVFX>> _vfxPool = new Dictionary<string, Stack<PooledVFX>>();
        private Transform _poolParent;

        private static readonly Dictionary<string, GameObject> PREFABS = new Dictionary<string, GameObject>();

        public void Initialize()
        {
            Transform parent = transform;

            _poolParent = new GameObject("VFXPool").transform;
            _poolParent.SetParent(parent, false);
        }

        public IPooledVFX CreateAt(string key, Vector3 worldPos)
        {
            PooledVFX vfx = CreateVFX(key);

            vfx.transform.SetParent(null);
            vfx.transform.position = worldPos;
            vfx.gameObject.SetActive(true);

            return vfx;
        }

        public IPooledVFX CreateAt(string key, Transform parent, Vector3 localPos, Quaternion localRot)
        {
            Debug.Assert(parent != null);

            PooledVFX vfx = CreateVFX(key);
            vfx.transform.SetParent(parent);
            vfx.transform.localPosition = localPos;
            vfx.transform.localRotation = localRot;
            vfx.gameObject.SetActive(true);

            return vfx;
        }

        private PooledVFX CreateVFX(string key)
        {
            if (!_vfxPool.TryGetValue(key, out Stack<PooledVFX> pool))
            {
                pool = new Stack<PooledVFX>();
                _vfxPool.Add(key, pool);
            }

            if (!pool.TryPop(out PooledVFX vfx))
            {
                vfx = Instantiate(GetPrefab(key)).AddComponent<PooledVFX>();
                vfx.Initialize(key, OnVFXStopped);
            }

            return vfx;
        }

        private GameObject GetPrefab(string key)
        {
            if (PREFABS.TryGetValue(key, out GameObject prefab))
            {
                return prefab;
            }

            GameObject newPrefab = Resources.Load<GameObject>($"VFX/{key}");
            PREFABS.Add(key, newPrefab);

            return newPrefab;
        }

        private void OnVFXStopped(PooledVFX vfx)
        {
            vfx.gameObject.SetActive(false);
            vfx.transform.SetParent(_poolParent);
            Debug.Assert(_vfxPool.ContainsKey(vfx.Key));
            _vfxPool[vfx.Key].Push(vfx);
        }
    }
}
