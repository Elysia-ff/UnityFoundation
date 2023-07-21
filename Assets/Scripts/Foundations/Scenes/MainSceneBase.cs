using Elysia.Inputs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Scenes
{
    public abstract class MainSceneBase : SceneBase
    {
        public Camera MainCamera { get; private set; }

        public InputManager Input { get; private set; }

        public override void Initialize(int handle)
        {
            base.Initialize(handle);

            MainCamera = Camera.main;

            Input = gameObject.AddComponent<InputManager>();
        }
    }
}
