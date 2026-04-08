using Elysia.UI;
using System.Collections;
using System.Collections.Generic;
using Elysia.VFX;
using UnityEngine;

namespace Elysia.Scenes
{
    public abstract class MainSceneBase : SceneBase
    {
        public Camera MainCamera { get; private set; }

        public UIManager UI { get; private set; }
        public VFXManager VFX { get; private set; }

        public override void Initialize(int handle)
        {
            base.Initialize(handle);

            MainCamera = Camera.main;

            UI = FindObjectOfType<UIManager>();
            UI.Initialize();
            VFX = gameObject.AddComponent<VFXManager>();
            VFX.Initialize();
        }
    }
}
