using Elysia.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Scenes
{
    public class UIScene : MainSceneBase
    {
        private float positionValue;

        private void OnGUI()
        {
            positionValue = GUI.HorizontalSlider(new Rect(20, 20, Screen.width - 40, 60), positionValue, 0, Enum.GetValues(typeof(EPosition)).Length - 1);
            positionValue = Mathf.FloorToInt(positionValue);

            EPosition pos = (EPosition)positionValue;
            GUI.Label(new Rect(20, 60, Screen.width - 40, 30), $"EPosition : {pos}");

            if (GUI.Button(new Rect(20, 100, Screen.width - 40, 60), $"Show {nameof(CommonWindow)}"))
            {
                Game.Scene.UI.ShowUI<CommonWindow>(pos, (commonWindow) =>
                {
                    commonWindow.OnClose += () => Debug.Log($"{nameof(CommonWindow)} hide");
                });
            }

            if (GUI.Button(new Rect(20, 180, Screen.width - 40, 60), $"Show {nameof(ModalWindow)}"))
            {
                Game.Scene.UI.ShowModalUI<ModalWindow>(null, pos);
            }
        }
    }
}
