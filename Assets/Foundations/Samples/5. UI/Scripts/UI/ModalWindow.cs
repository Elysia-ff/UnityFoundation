using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Elysia.UI
{
    public class ModalWindow : UIBase<ModalWindow>
    {
        [SerializeField] private Button _closeBtn;

        protected override void Initialize()
        {
            base.Initialize();

            _closeBtn.onClick.AddListener(Close);
        }
    }
}
