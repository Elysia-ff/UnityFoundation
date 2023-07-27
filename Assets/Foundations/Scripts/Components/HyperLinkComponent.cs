using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Elysia.Components
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class HyperLinkComponent : MonoBehaviour, IPointerClickHandler
    {
        public delegate void OnLinkClickedEvent(int index, in TMP_LinkInfo linkInfo);

        private TextMeshProUGUI _text;

        public event OnLinkClickedEvent OnLinkClicked;

        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            int index = TMP_TextUtilities.FindIntersectingLink(_text, eventData.position, Game.Scene.UI.Camera);
            if (index < 0)
            {
                return;
            }

            TMP_LinkInfo linkInfo = _text.textInfo.linkInfo[index];
            OnLinkClicked?.Invoke(index, linkInfo);
        }
    }
}
