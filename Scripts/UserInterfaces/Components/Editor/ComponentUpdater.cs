using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEditor;

namespace Elysia
{
    public static class ComponentUpdater
    {
        [MenuItem("CONTEXT/Image/Upgrade to ImageRef")]
        public static void UpgradeImage(MenuCommand command)
        {
            UpgradeComponent(command, @"Assets/Foundations/Scripts/UserInterfaces/Components/ImageRef.cs", "Upgrade Image Component");
        }

        [MenuItem("CONTEXT/Image/Upgrade to ImageRef", true)]
        public static bool UpgradeImageValidator(MenuCommand command)
        {
            return command.context.GetType() == typeof(UnityEngine.UI.Image);
        }

        [MenuItem("CONTEXT/SpriteRenderer/Add SpriteRendererRef")]
        public static void UpgradeSpriteRenderer(MenuCommand command)
        {
            if (command.context is not Component script || script.gameObject.TryGetComponent<SpriteRendererRef>(out _))
            {
                return;
            }

            Undo.RegisterCompleteObjectUndo(command.context, "Upgrade SpriteRenderer Component");

            script.gameObject.AddComponent<SpriteRendererRef>();
            EditorUtility.SetDirty(script.gameObject);
        }

        [MenuItem("CONTEXT/SpriteRenderer/Add SpriteRendererRef", true)]
        public static bool UpgradeSpriteRendererValidator(MenuCommand command)
        {
            return command.context is SpriteRenderer component && !component.TryGetComponent<SpriteRendererRef>(out _);
        }

        [MenuItem("CONTEXT/TextMeshProUGUI/Upgrade to Label")]
        public static void UpgradeTextMeshProUGUI(MenuCommand command)
        {
            SerializedObject serializedObject = UpgradeComponent(command, @"Assets/Foundations/Scripts/UserInterfaces/Components/Label.cs", "Upgrade TextMeshProUGUI Component");
            SerializedProperty textProperty = serializedObject.FindProperty("m_text");
            SerializedProperty fontProperty = serializedObject.FindProperty("m_fontAsset");
            SerializedProperty fontMaterialProperty = serializedObject.FindProperty("m_sharedMaterial");
            SerializedProperty fontStyleProperty = serializedObject.FindProperty("m_fontStyle");
            serializedObject.Update();
            textProperty.stringValue = string.Empty;
            fontProperty.objectReferenceValue = null;
            fontMaterialProperty.objectReferenceValue = null;
            fontStyleProperty.intValue = (int)FontStyles.Normal;
            serializedObject.ApplyModifiedProperties();

            Reflection.CallMethod(serializedObject.targetObject, "Awake");
            Reflection.CallMethod(serializedObject.targetObject, "OnEnable");

            Label label = (Label)serializedObject.targetObject;
            if (label.TryGetComponent(out LocalizeStringEvent localizeStringEvent))
            {
                label.StringReference.SetReference(localizeStringEvent.StringReference.TableReference, localizeStringEvent.StringReference.TableEntryReference);

                foreach (string key in localizeStringEvent.StringReference.Keys)
                {
                    label.StringReference.Add(key, localizeStringEvent.StringReference[key]);
                }

                label.StringReference.RefreshString();

                Object.DestroyImmediate(localizeStringEvent);
            }

            label.Rebuild(UnityEngine.UI.CanvasUpdate.PreRender);
        }

        [MenuItem("CONTEXT/TextMeshProUGUI/Upgrade to Label", true)]
        public static bool UpgradeTextMeshProUGUIValidator(MenuCommand command)
        {
            return command.context.GetType() == typeof(TextMeshProUGUI);
        }

        private static SerializedObject UpgradeComponent(MenuCommand command, string scriptPath, string undoMessage)
        {
            GameObject root = PrefabUtility.GetNearestPrefabInstanceRoot(command.context);
            if (root != null)
            {
                throw new System.InvalidOperationException($"{nameof(UpgradeComponent)}() cannot be done in prefab instance");
            }

            TextAsset script = AssetDatabase.LoadAssetAtPath<TextAsset>(scriptPath);

            Undo.RegisterCompleteObjectUndo(command.context, undoMessage);

            SerializedObject serializedObject = new SerializedObject(command.context);
            SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");
            serializedObject.Update();
            scriptProperty.objectReferenceValue = script;
            serializedObject.ApplyModifiedProperties();

            return serializedObject;
        }
    }
}
