using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Elysia.UI
{
    public class StatusWindow : UIBase<StatusWindow>
    {
        [SerializeField] private Button _closeBtn;

        public override void Initialize()
        {
            base.Initialize();

            _closeBtn.onClick.AddListener(Close);
        }
    }
}
