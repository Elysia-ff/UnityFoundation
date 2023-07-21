using Elysia;
using Elysia.Scenes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleSceneBase : MainSceneBase
{
    private void OnGUI()
    {
        if (GUI.Button(new Rect(20, 20, Screen.width - 40, 60), "Load Level0"))
        {
            Game.LoadScene("Level0", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        if (GUI.Button(new Rect(20, 100, Screen.width - 40, 60), "Load Level1"))
        {
            Game.LoadSceneAsync("Level1", UnityEngine.SceneManagement.LoadSceneMode.Single);
        }

        if (GUI.Button(new Rect(20, 180, Screen.width - 40, 60), "Load SubLevel0"))
        {
            Game.LoadScene("SubLevel0", UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }

        if (GUI.Button(new Rect(20, 260, Screen.width - 40, 60), "Unload SubLevel0"))
        {
            Game.UnloadSubSceneAsync(0);
        }
    }
}
