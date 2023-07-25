using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Elysia.UI
{
    public class HUD : HUDBase<HUD>, IHasLoadingBar
    {
        [SerializeField] private Button _button;
        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _text;

        protected override void Initialize()
        {
            base.Initialize();

            _button.onClick.AddListener(OnButton);
        }

        private void OnButton()
        {
            Debug.Log("Button clicked");
        }

        public void OnLoadingInProgress(float progress)
        {
            _slider.gameObject.SetActive(true);

            _slider.value = progress;
            _text.text = $"Loading : {(int)(progress * 100f + 0.5f)}";
        }
    }
}
