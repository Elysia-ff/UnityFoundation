using Elysia.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Elysia.Scenes
{
    public class AddressablesScene : MainSceneBase
    {
        public override void Initialize(int handle)
        {
            base.Initialize(handle);

            //Caching.ClearCache(); // Clear downloaded addressables caches to re-download them. use this for test purpose.
            //Addressables.GetDownloadSizeAsync("<key>"); // Get addressables size. returns 0 if no need to download. (e.g. cached already)
            //Addressables.DownloadDependenciesAsync("<key>"); // Download dependencies.
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(20, 20, Screen.width - 40, 60), "Instantiate"))
            {
                Addressables.InstantiateAsync("Prefabs/Cube.prefab");
            }
        }
    }
}
