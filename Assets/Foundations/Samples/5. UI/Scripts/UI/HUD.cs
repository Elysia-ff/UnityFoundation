using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Elysia.UI
{
    public class HUD : HUDBase<HUD>
    {
        [SerializeField] private Button _button;

        protected override void Initialize()
        {
            base.Initialize();

            _button.onClick.AddListener(OnButton);
        }

        private void OnButton()
        {
            Debug.Log("Button clicked");
        }
    }
}
