using System.Collections;
using System.Collections.Generic;
using Elysia.Inputs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Elysia
{
    public partial class InputManager : MonoBehaviour
    {
        private static InputManager _instance;

        [SerializeField] private EventSystem _eventSystem;
        [SerializeField] private InputSystemUIInputModule _inputModule;

        private UIInputActions _uiInputActions;
        public UIInputActions.UIActions UIActions => _uiInputActions.UI;

        private PointerEventData POINTER_EVENT;
        private readonly List<RaycastResult> RAYCAST_RESULTS = new List<RaycastResult>();

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            POINTER_EVENT = new PointerEventData(_eventSystem);

            DontDestroyOnLoad(gameObject);
        }

        public void Initialize()
        {
            _uiInputActions = new UIInputActions();
            _uiInputActions.UI.Enable();

            _inputModule.actionsAsset = _uiInputActions.asset;

            InitializeInternal();
        }

        public bool IsPointerOverUI(Vector2 position)
        {
            POINTER_EVENT.position = position;
            EventSystem.current.RaycastAll(POINTER_EVENT, RAYCAST_RESULTS);

            bool results = RAYCAST_RESULTS.Count > 0;
            RAYCAST_RESULTS.Clear();

            return results;
        }
    }
}
