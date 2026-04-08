using Elysia.Audios;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Elysia
{
    public partial class App : MonoBehaviour
    {
        private static App _instance;

        public static ResourceHolder ResourceHolder { get; } = new ResourceHolder().EnsureAllocated();
        public static AudioManager Audio { get; private set; }
        public static InputManager Input { get; private set; }

        private Task _audioInitializationTask;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif

            _instance = this;

            ResourceManager.ExceptionHandler = ExceptionHandler;
            Addressables.InitializeAsync();

            Audio = gameObject.AddComponent<AudioManager>();
            _audioInitializationTask = Audio.InitializeAsync();

            Input = FindObjectOfType<InputManager>();
            Input.Initialize();

            InitializeInternal();
            InitializeScenes();

            DontDestroyOnLoad(gameObject);
        }

        private static void ExceptionHandler(AsyncOperationHandle handle, Exception exception)
        {
            if (exception is InvalidKeyException)
            {
                throw exception;
            }

            Addressables.LogException(handle, exception);
        }
    }
}
