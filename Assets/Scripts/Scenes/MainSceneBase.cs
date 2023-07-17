using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Scenes
{
    public abstract class MainSceneBase : SceneBase
    {
        public Camera MainCamera { get; private set; }

        public override void Initialize(int handle)
        {
            base.Initialize(handle);

            MainCamera = Camera.main;
        }

        private void Update()
        {
            // DEBUGCODE
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    if (Input.GetKey(KeyCode.LeftAlt))
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
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Game.LoadSceneAsync("Level1", UnityEngine.SceneManagement.LoadSceneMode.Single);
            }
        }
    }
}
