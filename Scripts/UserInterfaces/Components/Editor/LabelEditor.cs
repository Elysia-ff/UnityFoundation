using System.Collections;
using System.Collections.Generic;
using Elysia.Localizations;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEditor;

namespace Elysia
{
    [CustomEditor(typeof(Label), true), CanEditMultipleObjects]
    public class LabelEditor : TMP_EditorPanelUI
    {
        private SerializedProperty _stringReferenceProperty;
        private SerializedProperty _fontPresetProperty;

        private bool _isFontAssetEmpty;

        protected override void OnEnable()
        {
            base.OnEnable();

            _stringReferenceProperty = this.FindProperty("_stringReference");
            _fontPresetProperty = this.FindProperty("_fontPreset");

            LocalizedAsset<FontPreset> asset = Reflection.GetField<LocalizedAsset<FontPreset>>(_fontPresetProperty.serializedObject.targetObject, _fontPresetProperty.propertyPath);
            _isFontAssetEmpty = asset.IsEmpty;
        }

        public override void OnInspectorGUI()
        {
            this.DrawEditorScriptHeader();

            // Make sure Multi selection only includes TMP Text objects.
            if (IsMixSelectionTypes())
            {
                return;
            }

            serializedObject.Update();

            DrawStringReference();

            DrawTextInput();

            DrawMainSettings();

            DrawExtraSettings();

            EditorGUILayout.Space();

            if (serializedObject.ApplyModifiedProperties() || m_HavePropertiesChanged)
            {
                m_TextComponent.havePropertiesChanged = true;
                m_HavePropertiesChanged = false;
                EditorUtility.SetDirty(target);
            }
        }

        protected override bool IsMixSelectionTypes()
        {
            GameObject[] objects = Selection.gameObjects;
            if (objects.Length > 1)
            {
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i].GetComponent<Label>() == null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void DrawStringReference()
        {
            EditorGUILayout.PropertyField(_stringReferenceProperty);

            LocalizedString str = _stringReferenceProperty.boxedValue as LocalizedString;
            if (str == null || str.IsEmpty)
            {
                EditorPropertyDriver.UnregisterProperty(m_TextComponent, "m_text");
            }
            else
            {
                EditorPropertyDriver.RegisterProperty(m_TextComponent, "m_text");
            }
        }

        private new void DrawMainSettings()
        {
            // MAIN SETTINGS SECTION
            GUILayout.Label(new GUIContent("<b>Main Settings</b>"), TMP_UIStyleManager.sectionHeader);

            //EditorGUI.indentLevel += 1;

            DrawFontPreset();
            if (_isFontAssetEmpty)
            {
                Reflection.CallMethod(this, "DrawFont");
            }
            else
            {
                DrawFontData();
            }

            EditorGUILayout.Space(10f);

            Reflection.CallMethod(this, "DrawColor");

            Reflection.CallMethod(this, "DrawSpacing");

            Reflection.CallMethod(this, "DrawAlignment");

            Reflection.CallMethod(this, "DrawWrappingOverflow");

            DrawTextureMapping();

            //EditorGUI.indentLevel -= 1;
        }

        private void DrawFontPreset()
        {
            string locale = LocalizationSettings.SelectedLocale != null ? LocalizationSettings.SelectedLocale.Identifier.Code : "None";
            {
                using var check = new EditorGUI.ChangeCheckScope();
                EditorGUILayout.PropertyField(_fontPresetProperty, new GUIContent($"{_fontPresetProperty.displayName} ({locale})"));

                if (check.changed)
                {
                    _fontPresetProperty.serializedObject.ApplyModifiedProperties();

                    LocalizedAsset<FontPreset> asset = Reflection.GetField<LocalizedAsset<FontPreset>>(_fontPresetProperty.serializedObject.targetObject, _fontPresetProperty.propertyPath);
                    if (!asset.IsEmpty)
                    {
                        FontPreset fontPreset = asset.LoadAsset();
                        if (FontPreset.ApplyTo(m_TextComponent, fontPreset))
                        {
                            m_HavePropertiesChanged = true;
                        }
                    }

                    _isFontAssetEmpty = asset.IsEmpty;
                }
            }
        }

        private void DrawFontData()
        {
            {
                using var _ = new EditorGUI.DisabledScope(true);
                EditorGUILayout.ObjectField("Font Asset", m_FontAssetProp.objectReferenceValue, typeof(TMP_FontAsset), false);
                EditorGUILayout.ObjectField("Font Material", m_FontSharedMaterialProp.objectReferenceValue, typeof(Material), false);
            }

            Rect rect;

            EditorGUI.indentLevel += 1;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_AutoSizingProp, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_AutoSizeLabel"));
            if (EditorGUI.EndChangeCheck())
            {
                if (m_AutoSizingProp.boolValue == false)
                    m_FontSizeProp.floatValue = m_FontSizeBaseProp.floatValue;

                m_HavePropertiesChanged = true;
            }

            // Show auto sizing options
            if (m_AutoSizingProp.boolValue)
            {
                rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);

                EditorGUI.PrefixLabel(rect, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_AutoSizeOptionsLabel"));

                int previousIndent = EditorGUI.indentLevel;

                EditorGUI.indentLevel = 0;

                rect.width = (rect.width - EditorGUIUtility.labelWidth) / 4f;
                rect.x += EditorGUIUtility.labelWidth;

                EditorGUIUtility.labelWidth = 24;
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(rect, m_FontSizeMinProp, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_MinLabel"));
                if (EditorGUI.EndChangeCheck())
                {
                    float minSize = m_FontSizeMinProp.floatValue;

                    minSize = Mathf.Max(0, minSize);

                    m_FontSizeMinProp.floatValue = Mathf.Min(minSize, m_FontSizeMaxProp.floatValue);
                    m_HavePropertiesChanged = true;
                }
                rect.x += rect.width;

                EditorGUIUtility.labelWidth = 27;
                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(rect, m_FontSizeMaxProp, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_MaxLabel"));
                if (EditorGUI.EndChangeCheck())
                {
                    float maxSize = Mathf.Clamp(m_FontSizeMaxProp.floatValue, 0, 32767);

                    m_FontSizeMaxProp.floatValue = Mathf.Max(m_FontSizeMinProp.floatValue, maxSize);
                    m_HavePropertiesChanged = true;
                }
                rect.x += rect.width;

                EditorGUI.BeginChangeCheck();
                EditorGUIUtility.labelWidth = 36;
                EditorGUI.PropertyField(rect, m_CharWidthMaxAdjProp, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_WdLabel"));
                rect.x += rect.width;
                EditorGUIUtility.labelWidth = 28;
                EditorGUI.PropertyField(rect, m_LineSpacingMaxProp, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_LineLabel"));

                EditorGUIUtility.labelWidth = 0;

                if (EditorGUI.EndChangeCheck())
                {
                    m_CharWidthMaxAdjProp.floatValue = Mathf.Clamp(m_CharWidthMaxAdjProp.floatValue, 0, 50);
                    m_LineSpacingMaxProp.floatValue = Mathf.Min(0, m_LineSpacingMaxProp.floatValue);
                    m_HavePropertiesChanged = true;
                }

                EditorGUI.indentLevel = previousIndent;
            }

            EditorGUI.indentLevel -= 1;

            EditorGUILayout.Space();
        }
    }
}
