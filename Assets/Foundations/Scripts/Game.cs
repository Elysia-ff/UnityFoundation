using Elysia.Audios;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public partial class Game : MonoBehaviour
    {
        private static Game _instance;

        public static AudioManager Audio { get; private set; }

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            Audio = gameObject.AddComponent<AudioManager>().Initialize();

            InitializeScenes();

            DontDestroyOnLoad(gameObject);
        }
    }
}
