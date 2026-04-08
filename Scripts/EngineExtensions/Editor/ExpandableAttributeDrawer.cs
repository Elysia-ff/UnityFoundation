using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Elysia
{
    // https://forum.unity.com/threads/editor-tool-better-scriptableobject-inspector-editing.484393/
    [CustomPropertyDrawer(typeof(ExpandableAttribute), true)]
    public class ExpandableAttributeDrawer : PropertyDrawer
    {
        private enum EBackgroundStyles
        {
            None,
            HelpBox,
            Darken,
            Lighten
        }

        /// <summary>
        /// Whether the default editor Script field should be shown.
        /// </summary>
        private static bool SHOW_SCRIPT_FIELD = false;

        /// <summary>
        /// The spacing on the inside of the background rect.
        /// </summary>
        private static float INNER_SPACING = 6.0f;

        /// <summary>
        /// The spacing on the outside of the background rect.
        /// </summary>
        private static float OUTER_SPACING = 4.0f;

        /// <summary>
        /// The style the background uses.
        /// </summary>
        private static EBackgroundStyles BACKGROUND_STYLE = EBackgroundStyles.HelpBox;

        /// <summary>
        /// The color that is used to darken the background.
        /// </summary>
        private static Color DARKEN_COLOUR = new Color(0.0f, 0.0f, 0.0f, 0.2f);

        /// <summary>
        /// The color that is used to lighten the background.
        /// </summary>
        private static Color LIGHTEN_COLOUR = new Color(1.0f, 1.0f, 1.0f, 0.2f);

        private static GUIStyle FOLD_OUT_STYLE;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var totalHeight = 0.0f;

            totalHeight += EditorGUIUtility.singleLineHeight;

            if (property.objectReferenceValue == null)
            {
                return totalHeight;
            }

            if (!property.isExpanded)
            {
                return totalHeight;
            }

            var targetObject = new SerializedObject(property.objectReferenceValue);
            // if (targetObject == null)
            // {
            //     return totalHeight;
            // }

            SerializedProperty field = targetObject.GetIterator();

            field.NextVisible(true);

            if (SHOW_SCRIPT_FIELD)
            {
                totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            while (field.NextVisible(false))
            {
                totalHeight += EditorGUI.GetPropertyHeight(field, true) + EditorGUIUtility.standardVerticalSpacing;
            }

            totalHeight += INNER_SPACING * 2;
            totalHeight += OUTER_SPACING * 2;

            return totalHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var fieldRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };

            if (property.objectReferenceValue == null)
            {
                EditorGUI.PropertyField(fieldRect, property, label, true);
                EditorGUI.EndProperty();
                return;
            }

            var indentedRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y,
                position.width - EditorGUIUtility.labelWidth, position.height);
            EditorGUI.PropertyField(indentedRect, property, GUIContent.none, true);

            if (FOLD_OUT_STYLE == null)
            {
                FOLD_OUT_STYLE = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold
                };
            }

            property.isExpanded = EditorGUI.Foldout(fieldRect, property.isExpanded, label, true, FOLD_OUT_STYLE);

            if (!property.isExpanded)
            {
                EditorGUI.EndProperty();
                return;
            }

            if (property.objectReferenceValue == null)
            {
                EditorGUI.EndProperty();
                return;
            }

            var targetObject = new SerializedObject(property.objectReferenceValue);
            // if (targetObject == null)
            // {
            //     EditorGUI.showMixedValue = previousShowMixedValue;
            //     return;
            // }

            #region Format Field Rects

            var propertyRects = new List<Rect>();
            var marchingRect = new Rect(fieldRect);

            var bodyRect = new Rect(fieldRect);
            bodyRect.xMin += EditorGUI.indentLevel * 14;
            bodyRect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing
                                                               + OUTER_SPACING;

            SerializedProperty field = targetObject.GetIterator();
            field.NextVisible(true);

            marchingRect.y += INNER_SPACING + OUTER_SPACING;

            if (SHOW_SCRIPT_FIELD)
            {
                propertyRects.Add(marchingRect);
                marchingRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            marchingRect.width -= 4f;
            while (field.NextVisible(false))
            {
                marchingRect.y += marchingRect.height + EditorGUIUtility.standardVerticalSpacing;
                marchingRect.height = EditorGUI.GetPropertyHeight(field, true);
                propertyRects.Add(marchingRect);
            }

            marchingRect.y += INNER_SPACING;

            bodyRect.yMax = marchingRect.yMax;

            #endregion

            DrawBackground(bodyRect);

            #region Draw Fields

            EditorGUI.indentLevel++;

            var index = 0;
            field = targetObject.GetIterator();
            field.NextVisible(true);

            if (SHOW_SCRIPT_FIELD)
            {
                // Show the disabled script field
                EditorGUI.BeginDisabledGroup(true);
                EditorGUI.PropertyField(propertyRects[index], field, true);
                EditorGUI.EndDisabledGroup();
                index++;
            }

            // Replacement for "editor.OnInspectorGUI();" so we have more control on how we draw the editor
            while (field.NextVisible(false))
            {
                try
                {
                    EditorGUI.PropertyField(propertyRects[index], field, true);
                }
                catch (StackOverflowException)
                {
                    field.objectReferenceValue = null;
                    Debug.LogError("Detected self-nesting causing a StackOverflowException, avoid using the same " +
                                   "object inside a nested structure.");
                }

                index++;
            }

            targetObject.ApplyModifiedProperties();

            EditorGUI.indentLevel--;

            #endregion

            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Draws the Background
        /// </summary>
        /// <param name="rect">The Rect where the background is drawn.</param>
        private void DrawBackground(Rect rect)
        {
            switch (BACKGROUND_STYLE)
            {
                case EBackgroundStyles.HelpBox:
                    EditorGUI.HelpBox(rect, "", MessageType.None);
                    break;

                case EBackgroundStyles.Darken:
                    EditorGUI.DrawRect(rect, DARKEN_COLOUR);
                    break;

                case EBackgroundStyles.Lighten:
                    EditorGUI.DrawRect(rect, LIGHTEN_COLOUR);
                    break;
            }
        }
    }
}
