using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Scenes
{
    public class TimelineScene : MainSceneBase
    {
        private Starter _starter;

        public override void Initialize(int handle)
        {
            base.Initialize(handle);

            _starter = FindObjectOfType<Starter>().Initialize();
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(20, 20, Screen.width - 40, 60), "Run"))
            {
                _starter.Run();
            }
        }
    }
}
