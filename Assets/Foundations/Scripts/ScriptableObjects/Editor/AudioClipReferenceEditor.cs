using System.Collections;
using System.Collections.Generic;
using Elysia.Animations;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

namespace Elysia
{
    [InitializeOnLoad]
    [CustomEditor(typeof(AudioClipReference))]
    public class AudioClipReferenceEditor : Editor
    {
        static AudioClipReferenceEditor()
        {
            AddressableAssetSettings.OnModificationGlobal -= OnAddressableModified;
            AddressableAssetSettings.OnModificationGlobal += OnAddressableModified;
        }

        private static void OnAddressableModified(AddressableAssetSettings settings, AddressableAssetSettings.ModificationEvent modificationEvent, object data)
        {
            if (modificationEvent != AddressableAssetSettings.ModificationEvent.EntryModified || data is not AddressableAssetEntry)
            {
                return;
            }

            string[] assetGUIDs = AssetDatabase.FindAssets($"t:{nameof(AudioClipReference)}");
            if (assetGUIDs.Length > 0)
            {
                string title = $"Refreshing {nameof(AudioClipReference)}s";
                const string infoFormat = "In progress... {0}/{1}";

                for (int i = 0; i < assetGUIDs.Length; i++)
                {
                    EditorUtility.DisplayProgressBar(title, string.Format(infoFormat, i + 1, assetGUIDs.Length), (float)(i + 1) / assetGUIDs.Length);

                    AudioClipReference audioClipReference = AssetDatabase.LoadAssetAtPath<AudioClipReference>(AssetDatabase.GUIDToAssetPath(assetGUIDs[i]));
                    audioClipReference.RefreshAssetKey();
                }

                EditorUtility.ClearProgressBar();
            }
        }

        private AudioClipReference _target;

        private void OnEnable()
        {
            _target = (AudioClipReference)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            DrawProperty("_data");
            _target.RefreshAssetKey();

            DrawProperty("_loop");
            if (_target.Loop)
            {
                DrawProperty("_stopMethod");

                if (_target.StopMethod == EStopMethod.OnAnimationContainsGivenStringStarted ||
                    _target.StopMethod == EStopMethod.OnAnimationDoesNotContainsGivenStringStarted)
                {
                    DrawProperty("_stringParameters");
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawProperty(string propertyName)
        {
            EditorGUILayout.PropertyField(FindProperty(propertyName));
        }

        private SerializedProperty FindProperty(string propertyName)
        {
            return serializedObject.FindProperty(propertyName);
        }
    }
}
