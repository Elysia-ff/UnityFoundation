using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Elysia.UI
{
    public class CommonWindow : UIBase<CommonWindow>
    {
        [SerializeField] private RectTransform _pivot;
        [SerializeField] private Button _closeBtn;
        [SerializeField] private TMP_Dropdown _positionDropdown;
        [SerializeField] private Button _showModalWindowBtn;

        protected override Vector2 Pivot => _pivot.localPosition;

        public event Action OnClose;

        protected override void Initialize()
        {
            base.Initialize();

            _closeBtn.onClick.AddListener(Close);

            _positionDropdown.ClearOptions();
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach (object k in Enum.GetValues(typeof(EPosition)))
            {
                options.Add(new TMP_Dropdown.OptionData(k.ToString()));
            }
            _positionDropdown.AddOptions(options);
            _positionDropdown.value = 0;

            _showModalWindowBtn.onClick.AddListener(OnShowModalWindow);
        }

        protected override void OnShow()
        {
            base.OnShow();

            Debug.Log($"{nameof(CommonWindow)} show");
        }

        protected override void OnHide()
        {
            base.OnHide();

            OnClose?.Invoke();
            OnClose = null;
        }

        private void OnShowModalWindow()
        {
            EPosition position = (EPosition)_positionDropdown.value;
            Game.Scene.UI.ShowModalUI<ModalWindow>(this, position);
        }
    }
}
