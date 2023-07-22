using Elysia.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Scenes
{
    public class UIScene : MainSceneBase
    {
        private void OnGUI()
        {
            if (GUI.Button(new Rect(20, 20, Screen.width - 40, 60), "Show StatusUI"))
            {
                StatusWindow statusUI = Game.Scene.UI.ShowUI<StatusWindow>();
            }
        }
    }
}
