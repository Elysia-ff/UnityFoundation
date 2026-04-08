using Elysia.Scenes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Elysia
{
    public partial class App
    {
        public static MainSceneBase Scene { get; private set; }
        public static T SceneAs<T>() where T : MainSceneBase => Scene as T;

        private readonly static List<SceneBase> _subScenes = new List<SceneBase>();
        public static IReadOnlyList<SceneBase> SubScenes => _subScenes;

        private static readonly Dictionary<string, Type> SCENE_TABLE = new Dictionary<string, Type>();

        public static void LoadSceneAsync(string key, ILoadingReceiver.Data receiver = null, LoadSceneMode mode = LoadSceneMode.Single)
        {
            _instance.StartCoroutine(LoadSceneAsyncRoutine(key, mode, receiver));
        }

        private static IEnumerator LoadSceneAsyncRoutine(string key, LoadSceneMode mode, ILoadingReceiver.Data loadingReceiver)
        {
            AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(key, mode, false);
            while (!handle.IsDone)
            {
                loadingReceiver?.SetProgress(handle.PercentComplete);

                yield return null;
            }

            loadingReceiver?.SetProgress(1f);

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                yield return handle.Result.ActivateAsync();
            }
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

        private async void OnActiveSceneChanged(Scene current, Scene next)
        {
            Debug.Assert(Scene == null || Scene.Handle != current.handle, "Changing active scene is not allowed.");

            await _audioInitializationTask;

            Scene = CreateScene<MainSceneBase>(next.name);
            if (Scene != null)
            {
                Scene.Initialize(next.handle);
            }
        }

        private T CreateScene<T>(string sceneName)
            where T : SceneBase
        {
            if (!SCENE_TABLE.TryGetValue(sceneName, out Type t))
            {
                t = Type.GetType($"Elysia.Scenes.{sceneName}");
                //Debug.Assert(t != null, $"Not found 'Elysia.Scenes.{sceneName}'");
                if (t == null)
                {
                    t = Type.GetType("Elysia.Scenes.PlayScene");
                    Debug.Assert(t != null);
                }

                SCENE_TABLE.Add(sceneName, t);
            }

            return new GameObject(sceneName, t).GetComponent<T>();
        }
    }
}
