using Elysia;
using Elysia.Scenes;
using Elysia.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleSceneBase : MainSceneBase
{
    public override void Initialize(int handle)
    {
        base.Initialize(handle);

        Game.Scene.UI.ShowHUD<HUD>();
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, Screen.width - 40, 60), "Load Level0"))
        {
            Game.LoadScene("Level0", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        if (GUI.Button(new Rect(20, 100, Screen.width - 40, 60), "Load Level1"))
        {
            Game.LoadSceneAsync("Level1", UnityEngine.SceneManagement.LoadSceneMode.Single, Game.Scene.UI.GetHUD<HUD>());
        }

        if (GUI.Button(new Rect(20, 180, Screen.width - 40, 60), "Load SubLevel0"))
        {
            Game.LoadSceneAsync("SubLevel0", UnityEngine.SceneManagement.LoadSceneMode.Additive, Game.Scene.UI.GetHUD<HUD>());
        }

        if (GUI.Button(new Rect(20, 260, Screen.width - 40, 60), "Unload SubLevel0"))
        {
            Game.UnloadSubSceneAsync(0, Game.Scene.UI.GetHUD<HUD>());
        }
    }
}
