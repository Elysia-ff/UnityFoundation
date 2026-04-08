using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Elysia
{
    [CustomPropertyDrawer(typeof(AsEnumAttribute))]
    public class AsEnumDrawer : PropertyDrawer
    {
        private int _selectedIndex = -1;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.serializedObject.isEditingMultipleObjects)
            {
                EditorGUI.LabelField(position, property.displayName, "Multiple objects editing not supported");
                return;
            }

            SerializedProperty stringValueProperty = property.FindPropertyRelative("_stringValue");
            if (stringValueProperty == null)
            {
                stringValueProperty = property;
            }

            AsEnumAttribute asEnumAttribute = (AsEnumAttribute)attribute;
            Type enumType = asEnumAttribute.enumType;
            List<string> displayOptions = new List<string> { "\t" };
            displayOptions.AddRange(Enum.GetNames(enumType));
            string[] names = displayOptions.ToArray();

            if (Enum.TryParse(enumType, stringValueProperty.stringValue, out object result))
            {
                _selectedIndex = Array.IndexOf(names, result.ToString());
            }
            else
            {
                _selectedIndex = -1;
            }

            _selectedIndex = EditorGUI.Popup(position, label.text, _selectedIndex, names);

            if (_selectedIndex >= 1 && _selectedIndex < names.Length)
            {
                stringValueProperty.stringValue = names[_selectedIndex];
            }
            else
            {
                stringValueProperty.stringValue = string.Empty;
            }
        }
    }
}
