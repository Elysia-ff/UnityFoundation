using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Elysia.Localizations
{
    [CreateAssetMenu(fileName = "FontPreset", menuName = "Foundations/Scriptable Objects/Font Preset")]
    public class FontPreset : ScriptableObject
    {
        [SerializeField] private TMP_FontAsset _fontAsset;
        public TMP_FontAsset FontAsset => _fontAsset;

        [SerializeField] private Material _fontMaterial;
        public Material FontMaterial => _fontMaterial;

        [SerializeField] private FontStyles _fontStyle = FontStyles.Normal;
        public FontStyles FontStyle => _fontStyle;

        [SerializeField] private float _fontSize = 36f;
        public float FontSize => _fontSize;

#if UNITY_EDITOR
        public static TMP_FontAsset GetDefaultFontAsset()
        {
            if (TMP_Settings.defaultFontAsset != null)
            {
                return TMP_Settings.defaultFontAsset;
            }

            return Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        }

        public static bool ApplyTo(TMP_Text text, FontPreset preset)
        {
            if (preset == null)
            {
                return false;
            }

            bool changed = false;

            if (Reflection.GetField<TMP_FontAsset>(text, "m_fontAsset") != preset._fontAsset)
            {
                Reflection.SetField(text, "m_fontAsset", preset._fontAsset);
                changed = true;
            }

            if (Reflection.GetField<Material>(text, "m_sharedMaterial") != preset._fontMaterial)
            {
                Reflection.SetField(text, "m_sharedMaterial", preset._fontMaterial);
                changed = true;
            }

            if (Reflection.GetField<FontStyles>(text, "m_fontStyle") != preset._fontStyle)
            {
                Reflection.SetField(text, "m_fontStyle", preset._fontStyle);
                changed = true;
            }

            if (!Reflection.GetField<bool>(text, "m_enableAutoSizing"))
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (Reflection.GetField<float>(text, "m_fontSize") != preset._fontSize)
                {
                    Reflection.SetField(text, "m_fontSize", preset._fontSize);
                    changed = true;
                }
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Reflection.GetField<float>(text, "m_fontSizeBase") != preset._fontSize)
            {
                Reflection.SetField(text, "m_fontSizeBase", preset._fontSize);
                changed = true;
            }

            return changed;
        }

        public static bool IsFontDataOverridden(TMP_Text text)
        {
            TMP_FontAsset defaultFont = GetDefaultFontAsset();

            if (Reflection.GetField<TMP_FontAsset>(text, "m_fontAsset") != defaultFont)
            {
                return false;
            }

            if (Reflection.GetField<Material>(text, "m_sharedMaterial") != defaultFont.material)
            {
                return false;
            }

            return true;
        }

        public static void NullifyFontData(TMP_Text text)
        {
            TMP_FontAsset defaultFont = GetDefaultFontAsset();
            Reflection.SetField(text, "m_fontAsset", defaultFont);
            Reflection.SetField(text, "m_sharedMaterial", defaultFont.material);
        }
#endif
    }
}
