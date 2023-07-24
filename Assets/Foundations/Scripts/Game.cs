using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public partial class Game : MonoBehaviour
    {
        private static Game _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            InitializeScenes();

            DontDestroyOnLoad(gameObject);
        }
    }
}
