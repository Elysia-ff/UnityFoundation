using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Elysia
{
    [CustomPropertyDrawer(typeof(AudioClipVolumeReference))]
    public class AudioClipVolumeReferenceDrawer : PropertyDrawer
    {
        private const float SPACING = 2f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + SPACING + 5f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect rect = position;

            rect.height = EditorGUIUtility.singleLineHeight;
            rect.width = position.width - EditorGUIUtility.singleLineHeight - SPACING;
            SerializedProperty clipProperty = property.FindPropertyRelative("_clip");
            EditorGUI.PropertyField(rect, clipProperty, label);

            rect.x += rect.width + SPACING;
            rect.width = EditorGUIUtility.singleLineHeight;
            if (GUI.Button(rect, AudioUtil.PlayTexture, EditorStyles.iconButton))
            {
                if (AudioUtil.IsPreviewClipPlaying())
                {
                    AudioUtil.StopAllPreviewClips();
                }
                else
                {
                    AudioClipReference reference = (AudioClipReference)clipProperty.boxedValue;
                    AudioUtil.PlayPreviewClip(reference.editorAsset);
                }
            }

            using EditorGUI.IndentLevelScope _ = new EditorGUI.IndentLevelScope();
            rect.x = position.x;
            rect.y += rect.height + SPACING;
            rect.width = position.width;
            SerializedProperty volumeProperty = property.FindPropertyRelative("_volume");
            EditorGUI.Slider(rect, volumeProperty, 0f, 1f);
        }
    }
}
