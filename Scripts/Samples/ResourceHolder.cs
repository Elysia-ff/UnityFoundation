#if false

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public partial class ResourceHolder
    {
        private readonly Dictionary<StringID, Projectile> _projectiles = new Dictionary<StringID, Projectile>();
        public IReadOnlyDictionary<StringID, Projectile> Projectiles => _projectiles;

        public bool IsLoaded { get; private set; }

        public void LoadAll()
        {
            MakeLoadingProcess();
            LoadAssets();

            IsLoaded = true;
        }

        public IEnumerator LoadAllAsync(ILoadingReceiver.Data loadingReceiver)
        {
            MakeLoadingProcess();
            yield return LoadAssetsAsync(loadingReceiver);

            IsLoaded = true;
        }

        private void MakeLoadingProcess()
        {
            LoadObject<GameObject, Projectile>(0, "projectiles", obj => obj.GetComponent<Projectile>(), OnProjectileLoaded);
        }

        private void OnProjectileLoaded(System.Guid guid, string key, Projectile asset)
        {
            _projectiles.Add((StringID)key, asset);
        }
    }
}

#endif
