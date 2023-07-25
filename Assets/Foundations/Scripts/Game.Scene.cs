using Elysia.Scenes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Elysia
{
    public partial class Game
    {
        public static MainSceneBase Scene { get; private set; }
        public static T SceneAs<T>() where T : MainSceneBase => (T)Scene;

        private static List<SceneBase> _subScenes = new List<SceneBase>();
        public static IReadOnlyList<SceneBase> SubScenes => _subScenes;

        private static readonly Dictionary<string, Type> SCENE_TABLE = new Dictionary<string, Type>();

        /// <summary>
        /// if <paramref name="mode"/> == Single, unload all scenes and load <paramref name="sceneName"/> as MainScene.
        /// </summary>
        public static void LoadScene(string sceneName, LoadSceneMode mode)
        {
            SceneManager.LoadScene(sceneName, mode);
        }

        /// <summary>
        /// Async version of LoadScene. See its summary for more detail.
        /// </summary>
        public static void LoadSceneAsync(string sceneName, LoadSceneMode mode, IHasLoadingBar receiver = null)
        {
            _instance.StartCoroutine(LoadSceneAsyncRoutine(sceneName, mode, receiver));
        }

        public static void UnloadSubSceneAsync(int index, IHasLoadingBar receiver = null)
        {
            Debug.Assert(0 <= index && index < _subScenes.Count);

            int handle = _subScenes[index].Handle;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene s = SceneManager.GetSceneAt(i);
                if (s.handle == handle)
                {
                    _instance.StartCoroutine(UnloadSceneAsyncRoutine(s, receiver));
                    return;
                }
            }

            Debug.Assert(false, "unreachable");
        }

        private void InitializeScenes()
        {
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void OnSceneUnloaded(Scene scene)
        {
            int handle = scene.handle;
            if (Scene != null && Scene.Handle == handle)
            {
                Scene = null;
            }
            else
            {
                int idx = _subScenes.FindIndex(s => s.Handle == handle);
                if (0 <= idx && idx < _subScenes.Count)
                {
                    _subScenes.RemoveAt(idx);
                }
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (mode != LoadSceneMode.Additive)
            {
                return;
            }

            SceneBase sceneBase = CreateScene<SceneBase>(scene.name);
            SceneManager.MoveGameObjectToScene(sceneBase.gameObject, scene);
            sceneBase.Initialize(scene.handle);
            _subScenes.Add(sceneBase);
        }

        private void OnActiveSceneChanged(Scene current, Scene next)
        {
            Debug.Assert(Scene == null || Scene.Handle != current.handle, $"Changing active scene is not allowed. Use {nameof(LoadScene)} or {nameof(LoadSceneAsync)}");

            Scene = CreateScene<MainSceneBase>(next.name);
            Scene.Initialize(next.handle);
        }

        private T CreateScene<T>(string sceneName)
            where T : SceneBase
        {
            if (!SCENE_TABLE.TryGetValue(sceneName, out Type t))
            {
                t = Type.GetType($"Elysia.Scenes.{sceneName}");
                Debug.Assert(t != null, $"Not found 'Elysia.Scenes.{sceneName}'");
                SCENE_TABLE.Add(sceneName, t);
            }

            return new GameObject(sceneName, t).GetComponent<T>();
        }

        private static IEnumerator LoadSceneAsyncRoutine(string sceneName, LoadSceneMode mode, IHasLoadingBar receiver)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, mode);

            while (!op.isDone)
            {
                if (receiver != null)
                {
                    float progress = op.progress;
                    receiver.OnLoadingInProgress(progress);
                }

                yield return null;
            }

            if (mode == LoadSceneMode.Additive && receiver != null)
            {
                receiver.OnLoadingInProgress(1f);
            }
        }

        private static IEnumerator UnloadSceneAsyncRoutine(Scene scene, IHasLoadingBar receiver)
        {
            AsyncOperation op = SceneManager.UnloadSceneAsync(scene);

            while (!op.isDone)
            {
                if (receiver != null)
                {
                    float progress = op.progress;
                    receiver.OnLoadingInProgress(progress);
                }

                yield return null;
            }

            if (receiver != null)
            {
                receiver.OnLoadingInProgress(1f);
            }
        }
    }
}
