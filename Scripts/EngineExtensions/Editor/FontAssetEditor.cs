using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEditor;

namespace Elysia
{
    [CustomEditor(typeof(TMP_FontAsset))]
    public class FontAssetEditor : TMP_FontAssetEditor
    {
        private TMP_FontAsset _fontAsset;
        private MonoScript _postProcessorScript;

        private new void OnEnable()
        {
            base.OnEnable();

            _fontAsset = (TMP_FontAsset)target;

            string[] scriptGuids = AssetDatabase.FindAssets($"t:MonoScript {nameof(FontAssetPostProcessor)}");
            Debug.Assert(scriptGuids != null && scriptGuids.Length == 1);
            _postProcessorScript = AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(scriptGuids[0]));
        }

        public override void OnInspectorGUI()
        {
            this.DrawEditorScriptHeader();

            {
                using var _ = new EditorGUI.DisabledScope(true);
                EditorGUILayout.ObjectField("Postprocessor", _postProcessorScript, typeof(FontAssetPostProcessor), false);
            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Dynamic Data Setting", EditorStyles.boldLabel);
            {
                Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight + 2f);
                rect.width = Mathf.Max(25f, rect.width / 2f);

                bool prevValue = FontAssetPostProcessor.GetClearDynamicData();
                bool newValue = prevValue;

                if (TMP_EditorUtility.EditorToggle(rect, newValue, new GUIContent("Ignore Changes"), EditorStyles.miniButton))
                {
                    newValue = true;
                }

                rect.x += rect.width;
                if (TMP_EditorUtility.EditorToggle(rect, !newValue, new GUIContent("Keep Changes"), EditorStyles.miniButton))
                {
                    newValue = false;
                }

                if (prevValue != newValue)
                {
                    FontAssetPostProcessor.SetClearDynamicData(newValue);
                }
            }
            EditorGUILayout.Space();

            base.OnInspectorGUI();
        }
    }
}
