using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Elysia.UI
{
    public class StatusWindow : UIBase<StatusWindow>
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _showModalWindowBtn;

        protected override void Initialize()
        {
            base.Initialize();

            _closeBtn.onClick.AddListener(Close);
            _showModalWindowBtn.onClick.AddListener(OnShowModalWindow);
        }

        private void OnShowModalWindow()
        {
            Game.Scene.UI.ShowModalUI<ModalWindow>(this);
        }
    }
}
