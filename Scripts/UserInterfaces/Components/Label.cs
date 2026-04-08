using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using Elysia.Localizations;

namespace Elysia
{
    [ExecuteInEditMode]
    public class Label : TextMeshProUGUI
    {
        [SerializeField] private LocalizedString _stringReference = new LocalizedString();
        public LocalizedString StringReference => _stringReference;

        [SerializeField] private LocalizedAsset<FontPreset> _fontPreset = new LocalizedAsset<FontPreset>();

        private LocaleIdentifier _currentLocale;
        private readonly LocalizedAsset<FontPreset>.ChangeHandler _onFontPresetChanged;
        private readonly LocalizedString.ChangeHandler _onStringReferenceChanged;

        public Label()
        {
            _onFontPresetChanged = OnFontPresetChanged;
            _onStringReferenceChanged = OnStringReferenceChanged;
        }

        protected override void OnEnable()
        {
            if (!m_isAwake)
            {
                return;
            }

            if (!_stringReference.IsEmpty)
            {
                m_text = null;
            }

            base.OnEnable();

            _fontPreset.AssetChanged += _onFontPresetChanged;
            _stringReference.StringChanged += _onStringReferenceChanged;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            _fontPreset.AssetChanged -= _onFontPresetChanged;
            _stringReference.StringChanged -= _onStringReferenceChanged;
        }

        public void SetTextAfterFontLoaded(string sourceText)
        {
            if (_fontPreset.IsEmpty)
            {
                SetText(sourceText);
            }
            else
            {
                AsyncOperationHandle<FontPreset> handle = _fontPreset.CurrentLoadingOperationHandle;
                if (handle.IsDone)
                {
                    SetText(sourceText);
                }
                else
                {
                    handle.Completed += _ =>
                    {
                        SetText(sourceText);
                    };
                }
            }
        }

        public void SetTextAfterFontLoaded(System.Text.StringBuilder sourceText)
        {
            if (_fontPreset.IsEmpty)
            {
                SetText(sourceText);
            }
            else
            {
                AsyncOperationHandle<FontPreset> handle = _fontPreset.CurrentLoadingOperationHandle;
                if (handle.IsDone)
                {
                    SetText(sourceText);
                }
                else
                {
                    handle.Completed += _ =>
                    {
                        SetText(sourceText);
                    };
                }
            }
        }

        public void SetTextAfterFontLoaded(char[] sourceText, int start, int length)
        {
            if (_fontPreset.IsEmpty)
            {
                SetText(sourceText, start, length);
            }
            else
            {
                AsyncOperationHandle<FontPreset> handle = _fontPreset.CurrentLoadingOperationHandle;
                if (handle.IsDone)
                {
                    SetText(sourceText, start, length);
                }
                else
                {
                    handle.Completed += _ =>
                    {
                        SetText(sourceText, start, length);
                    };
                }
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 프리팹이 저장될 때 font data 를 기본 상태로 유지
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();

            if (TMP_Settings.instance == null)
            {
                return;
            }

            _stringReference.RefreshString();

            if (!_fontPreset.IsEmpty)
            {
                m_fontAsset = FontPreset.GetDefaultFontAsset();
                m_sharedMaterial = m_fontAsset.material;
            }
        }
#endif

        private void OnFontPresetChanged(FontPreset value)
        {
            if (value == null)
            {
                return;
            }

            // locale 이 변경된 경우 기존 text 에 의해 새 locale 의 font atlas 가 갱신되는 것을 방지
            if (!_stringReference.IsEmpty && _currentLocale != LocalizationSettings.SelectedLocale.Identifier)
            {
                text = null;
            }

            m_fontAsset = value.FontAsset;
            m_sharedMaterial = value.FontMaterial;
            fontStyle = value.FontStyle;
            fontSize = value.FontSize;
            m_havePropertiesChanged = true;
            LoadFontAsset();
        }

        private void OnStringReferenceChanged(string value)
        {
            if (_stringReference.IsEmpty)
            {
                _currentLocale = default;
                return;
            }

            AsyncOperationHandle<FontPreset> handle = _fontPreset.CurrentLoadingOperationHandle;
            if (handle.IsDone)
            {
                text = value;
                _currentLocale = LocalizationSettings.SelectedLocale.Identifier;
            }
            else
            {
                handle.Completed += _ =>
                {
                    text = value;
                    _currentLocale = LocalizationSettings.SelectedLocale.Identifier;
                };
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("CONTEXT/Label/Localize")]
        public static void DisableLabelLocalize()
        {
            // do nothing,
        }

        [UnityEditor.MenuItem("CONTEXT/Label/Localize", true)]
        public static bool DisableLabelLocalizeValidator()
        {
            return false;
        }
#endif
    }
}
