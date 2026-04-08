using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEditor;

namespace Elysia.Localizations
{
    [CustomEditor(typeof(FontPreset))]
    public class FontPresetEditor : Editor
    {
        protected bool _havePropertiesChanged;

        protected SerializedProperty _fontAssetProperty;
        protected SerializedProperty _fontMaterialProperty;
        protected SerializedProperty _fontStyleProperty;
        protected SerializedProperty _fontSizeProperty;

        protected Material[] _materialPresets;
        protected GUIContent[] _materialPresetNames;
        protected readonly Dictionary<int, int> _materialPresetIndexLookup = new Dictionary<int, int>();
        protected int _materialPresetSelectionIndex;

        protected virtual void OnEnable()
        {
            _fontAssetProperty = this.FindProperty("_fontAsset");
            _fontMaterialProperty = this.FindProperty("_fontMaterial");
            _fontStyleProperty = serializedObject.FindProperty("_fontStyle");
            _fontSizeProperty = serializedObject.FindProperty("_fontSize");

            _materialPresetNames = GetMaterialPresets();
        }

        public override void OnInspectorGUI()
        {
            this.DrawScriptHeader();

            serializedObject.Update();

            DrawFont();

            if (serializedObject.ApplyModifiedProperties() || _havePropertiesChanged)
            {
                _havePropertiesChanged = false;
                EditorUtility.SetDirty(target);
            }
        }

        protected void DrawFont()
        {
            bool isFontAssetDirty = false;

            // FONT ASSET
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_fontAssetProperty, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_FontAssetLabel"));
            if (EditorGUI.EndChangeCheck())
            {
                _havePropertiesChanged = true;

                // Get new Material Presets for the new font asset
                _materialPresetNames = GetMaterialPresets();
                _materialPresetSelectionIndex = 0;
                _fontMaterialProperty.objectReferenceValue = _materialPresets[_materialPresetSelectionIndex];

                isFontAssetDirty = true;
            }

            Rect rect;

            // MATERIAL PRESET
            if (_materialPresetNames != null && !isFontAssetDirty)
            {
                EditorGUI.BeginChangeCheck();
                rect = EditorGUILayout.GetControlRect(false, 17);

                EditorGUI.BeginProperty(rect, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_MaterialPresetLabel"), _fontMaterialProperty);

                float oldHeight = EditorStyles.popup.fixedHeight;
                EditorStyles.popup.fixedHeight = rect.height;

                int oldSize = EditorStyles.popup.fontSize;
                EditorStyles.popup.fontSize = 11;

                if (_fontMaterialProperty.objectReferenceValue != null)
                {
                    _materialPresetIndexLookup.TryGetValue(_fontMaterialProperty.objectReferenceValue.GetInstanceID(), out _materialPresetSelectionIndex);
                }

                _materialPresetSelectionIndex = EditorGUI.Popup(rect, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_MaterialPresetLabel"), _materialPresetSelectionIndex, _materialPresetNames);

                EditorGUI.EndProperty();

                if (EditorGUI.EndChangeCheck())
                {
                    _fontMaterialProperty.objectReferenceValue = _materialPresets[_materialPresetSelectionIndex];
                    _havePropertiesChanged = true;
                }

                EditorStyles.popup.fixedHeight = oldHeight;
                EditorStyles.popup.fontSize = oldSize;
            }

            // FONT STYLE
            EditorGUI.BeginChangeCheck();

            int v1, v2, v3, v4, v5, v6, v7;

            if (EditorGUIUtility.wideMode)
            {
                rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 2f);

                EditorGUI.BeginProperty(rect, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_FontStyleLabel"), _fontStyleProperty);

                EditorGUI.PrefixLabel(rect, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_FontStyleLabel"));

                int styleValue = _fontStyleProperty.intValue;

                rect.x += EditorGUIUtility.labelWidth;
                rect.width -= EditorGUIUtility.labelWidth;

                rect.width = Mathf.Max(25f, rect.width / 7f);

                v1 = TMP_EditorUtility.EditorToggle(rect, (styleValue & 1) == 1, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_BoldLabel"), TMP_UIStyleManager.alignmentButtonLeft) ? 1 : 0; // Bold
                rect.x += rect.width;
                v2 = TMP_EditorUtility.EditorToggle(rect, (styleValue & 2) == 2, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_ItalicLabel"), TMP_UIStyleManager.alignmentButtonMid) ? 2 : 0; // Italics
                rect.x += rect.width;
                v3 = TMP_EditorUtility.EditorToggle(rect, (styleValue & 4) == 4, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_UnderlineLabel"), TMP_UIStyleManager.alignmentButtonMid) ? 4 : 0; // Underline
                rect.x += rect.width;
                v7 = TMP_EditorUtility.EditorToggle(rect, (styleValue & 64) == 64, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_StrikethroughLabel"), TMP_UIStyleManager.alignmentButtonRight) ? 64 : 0; // Strikethrough
                rect.x += rect.width;

                int selected = 0;

                EditorGUI.BeginChangeCheck();
                v4 = TMP_EditorUtility.EditorToggle(rect, (styleValue & 8) == 8, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_LowercaseLabel"), TMP_UIStyleManager.alignmentButtonLeft) ? 8 : 0; // Lowercase
                if (EditorGUI.EndChangeCheck() && v4 > 0)
                {
                    selected = v4;
                }
                rect.x += rect.width;
                EditorGUI.BeginChangeCheck();
                v5 = TMP_EditorUtility.EditorToggle(rect, (styleValue & 16) == 16, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_UppercaseLabel"), TMP_UIStyleManager.alignmentButtonMid) ? 16 : 0; // Uppercase
                if (EditorGUI.EndChangeCheck() && v5 > 0)
                {
                    selected = v5;
                }
                rect.x += rect.width;
                EditorGUI.BeginChangeCheck();
                v6 = TMP_EditorUtility.EditorToggle(rect, (styleValue & 32) == 32, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_SmallcapsLabel"), TMP_UIStyleManager.alignmentButtonRight) ? 32 : 0; // Smallcaps
                if (EditorGUI.EndChangeCheck() && v6 > 0)
                {
                    selected = v6;
                }

                if (selected > 0)
                {
                    v4 = selected == 8 ? 8 : 0;
                    v5 = selected == 16 ? 16 : 0;
                    v6 = selected == 32 ? 32 : 0;
                }

                EditorGUI.EndProperty();
            }
            else
            {
                rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 2f);

                EditorGUI.BeginProperty(rect, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_FontStyleLabel"), _fontStyleProperty);

                EditorGUI.PrefixLabel(rect, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_FontStyleLabel"));

                int styleValue = _fontStyleProperty.intValue;

                rect.x += EditorGUIUtility.labelWidth;
                rect.width -= EditorGUIUtility.labelWidth;
                rect.width = Mathf.Max(25f, rect.width / 4f);

                v1 = TMP_EditorUtility.EditorToggle(rect, (styleValue & 1) == 1, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_BoldLabel"), TMP_UIStyleManager.alignmentButtonLeft) ? 1 : 0; // Bold
                rect.x += rect.width;
                v2 = TMP_EditorUtility.EditorToggle(rect, (styleValue & 2) == 2, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_ItalicLabel"), TMP_UIStyleManager.alignmentButtonMid) ? 2 : 0; // Italics
                rect.x += rect.width;
                v3 = TMP_EditorUtility.EditorToggle(rect, (styleValue & 4) == 4, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_UnderlineLabel"), TMP_UIStyleManager.alignmentButtonMid) ? 4 : 0; // Underline
                rect.x += rect.width;
                v7 = TMP_EditorUtility.EditorToggle(rect, (styleValue & 64) == 64, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_StrikethroughLabel"), TMP_UIStyleManager.alignmentButtonRight) ? 64 : 0; // Strikethrough

                rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 2f);

                rect.x += EditorGUIUtility.labelWidth;
                rect.width -= EditorGUIUtility.labelWidth;

                rect.width = Mathf.Max(25f, rect.width / 4f);

                int selected = 0;

                EditorGUI.BeginChangeCheck();
                v4 = TMP_EditorUtility.EditorToggle(rect, (styleValue & 8) == 8, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_LowercaseLabel"), TMP_UIStyleManager.alignmentButtonLeft) ? 8 : 0; // Lowercase
                if (EditorGUI.EndChangeCheck() && v4 > 0)
                {
                    selected = v4;
                }
                rect.x += rect.width;
                EditorGUI.BeginChangeCheck();
                v5 = TMP_EditorUtility.EditorToggle(rect, (styleValue & 16) == 16, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_UppercaseLabel"), TMP_UIStyleManager.alignmentButtonMid) ? 16 : 0; // Uppercase
                if (EditorGUI.EndChangeCheck() && v5 > 0)
                {
                    selected = v5;
                }
                rect.x += rect.width;
                EditorGUI.BeginChangeCheck();
                v6 = TMP_EditorUtility.EditorToggle(rect, (styleValue & 32) == 32, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_SmallcapsLabel"), TMP_UIStyleManager.alignmentButtonRight) ? 32 : 0; // Smallcaps
                if (EditorGUI.EndChangeCheck() && v6 > 0)
                {
                    selected = v6;
                }

                if (selected > 0)
                {
                    v4 = selected == 8 ? 8 : 0;
                    v5 = selected == 16 ? 16 : 0;
                    v6 = selected == 32 ? 32 : 0;
                }

                EditorGUI.EndProperty();
            }

            if (EditorGUI.EndChangeCheck())
            {
                _fontStyleProperty.intValue = v1 + v2 + v3 + v4 + v5 + v6 + v7;
                _havePropertiesChanged = true;
            }

            // FONT SIZE
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_fontSizeProperty, Reflection.GetField<TMP_BaseEditorPanel, GUIContent>("k_FontSizeLabel"), GUILayout.MaxWidth(EditorGUIUtility.labelWidth + 50f));

            if (EditorGUI.EndChangeCheck())
            {
                float fontSize = Mathf.Clamp(_fontSizeProperty.floatValue, 0, 32767);

                _fontSizeProperty.floatValue = fontSize;
                _havePropertiesChanged = true;
            }
        }

        protected GUIContent[] GetMaterialPresets()
        {
            TMP_FontAsset fontAsset = _fontAssetProperty.objectReferenceValue as TMP_FontAsset;
            if (fontAsset == null)
            {
                return null;
            }

            _materialPresets = TMP_EditorUtility.FindMaterialReferences(fontAsset);
            _materialPresetNames = new GUIContent[_materialPresets.Length];

            _materialPresetIndexLookup.Clear();

            for (int i = 0; i < _materialPresetNames.Length; i++)
            {
                _materialPresetNames[i] = new GUIContent(_materialPresets[i].name);

                _materialPresetIndexLookup.Add(_materialPresets[i].GetInstanceID(), i);

                //if (m_TargetMaterial.GetInstanceID() == _materialPresets[i].GetInstanceID())
                //    m_MaterialPresetSelectionIndex = i;
            }

            return _materialPresetNames;
        }
    }
}
