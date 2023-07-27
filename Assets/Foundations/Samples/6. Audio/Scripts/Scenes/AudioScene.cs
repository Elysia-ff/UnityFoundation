using Elysia.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Scenes
{
    public class AudioScene : MainSceneBase
    {
        public override void Initialize(int handle)
        {
            base.Initialize(handle);

            Game.Scene.UI.ShowHUD<AudioHUD>();
        }
    }
}
