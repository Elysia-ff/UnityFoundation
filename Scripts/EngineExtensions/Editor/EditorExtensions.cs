using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Elysia
{
    public static class EditorExtensions
    {
        public static void DrawProperty(this Editor editor, string propertyName)
        {
            EditorGUILayout.PropertyField(editor.FindProperty(propertyName));
        }

        public static void DrawProperty(this Editor editor, string propertyName, string title)
        {
            EditorGUILayout.PropertyField(editor.FindProperty(propertyName), new GUIContent(title));
        }

        public static SerializedProperty FindProperty(this Editor editor, string propertyName)
        {
            return editor.serializedObject.FindProperty(propertyName);
        }

        public static bool HasElement(this SerializedProperty property, System.Predicate<SerializedProperty> predicate)
        {
            Debug.Assert(property.isArray);

            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty element = property.GetArrayElementAtIndex(i);
                if (predicate(element))
                {
                    return true;
                }
            }

            return false;
        }

        public static void DrawScriptHeader(this Editor editor)
        {
            DrawEditorScriptHeader(editor);

            using var _ = new EditorGUI.DisabledScope(true);
            editor.DrawProperty("m_Script");
        }

        public static void DrawEditorScriptHeader(this Editor editor)
        {
            using var _ = new EditorGUI.DisabledScope(true);
            EditorGUILayout.ObjectField("Editor Script", MonoScript.FromScriptableObject(editor), editor.GetType(), false);
        }
    }
}
