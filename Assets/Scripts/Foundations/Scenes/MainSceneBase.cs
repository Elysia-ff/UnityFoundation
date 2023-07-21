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

        protected virtual void Update()
        {
            // DEBUGCODE
            if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (UnityEngine.Input.GetKey(KeyCode.LeftShift))
                {
                    if (UnityEngine.Input.GetKey(KeyCode.LeftAlt))
                    {
                        Game.UnloadSubSceneAsync(0);
                    }
                    else
                    {
                        Game.LoadScene("SubLevel0", UnityEngine.SceneManagement.LoadSceneMode.Additive);
                    }
                }
                else
                {
                    Game.LoadScene("Level0", UnityEngine.SceneManagement.LoadSceneMode.Single);
                }
            }
            else if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
            {
                Game.LoadSceneAsync("Level1", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
        }
    }
}
