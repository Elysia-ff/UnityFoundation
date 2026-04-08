using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.AnalyzeRules;
using UnityEditor.AddressableAssets.Settings;

namespace Elysia.AnalyzeRules
{
    public class RemoveDependencyRule : AnalyzeRule
    {
        private struct Result
        {
            public string path;
            public object component;
        }

        public override string ruleName => "Remove Addressable Dependencies";

        public override bool CanFix => true;

        private List<Result> _results;

        public override void FixIssues(AddressableAssetSettings settings)
        {
            if (_results == null || _results.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _results.Count; i++)
            {
                FixIssue(_results[i]);

                GUID guid = AssetDatabase.GUIDFromAssetPath(_results[i].path);
                AssetDatabase.SaveAssetIfDirty(guid);
            }
        }

        private void FixIssue(Result result)
        {
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(result.path);
            if (obj == null)
            {
                return;
            }

            if (result.component is ImageRef imageRef)
            {
                imageRef.sprite = null;
                EditorUtility.SetDirty(obj);
            }
            else if (result.component is SpriteRendererRef spriteRendererRef)
            {
                spriteRendererRef.Component.sprite = null;
                EditorUtility.SetDirty(obj);
            }
        }

        public override List<AnalyzeResult> RefreshAnalysis(AddressableAssetSettings settings)
        {
            ClearAnalysis();

            string[] assets = AssetDatabase.GetAllAssetPaths();

            _results = new List<Result>(128);

            for (int i = 0; i < assets.Length; i++)
            {
                string assetPath = assets[i];
                Type mainAssetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                if (mainAssetType != typeof(GameObject))
                {
                    continue;
                }

                GameObject obj = (GameObject)AssetDatabase.LoadMainAssetAtPath(assetPath);
                CollectInvalidImageRef(obj, _results);
                CollectInvalidSpriteRendererRef(obj, _results);
            }

            if (_results.Count == 0)
            {
                return new List<AnalyzeResult> { noErrors };
            }

            return _results.Select(e => new AnalyzeResult
            {
                resultName = $"{e.component.GetType().Name}{kDelimiter}{e.path}",
                severity = MessageType.Warning
            }).ToList();
        }

        public override void ClearAnalysis()
        {
            base.ClearAnalysis();

            _results = null;
        }

        private void CollectInvalidImageRef(GameObject parent, List<Result> result)
        {
            ImageRef[] components = parent.GetComponentsInChildren<ImageRef>(true);
            for (int i = 0; i < components.Length; i++)
            {
                ImageRef component = components[i];
                if (component.sprite == null)
                {
                    continue;
                }

                result.Add(new Result
                {
                    path = AssetDatabase.GetAssetPath(parent),
                    component = component
                });
            }
        }

        private void CollectInvalidSpriteRendererRef(GameObject parent, List<Result> result)
        {
            SpriteRendererRef[] components = parent.GetComponentsInChildren<SpriteRendererRef>(true);
            for (int i = 0; i < components.Length; i++)
            {
                SpriteRendererRef component = components[i];
                if (component.Component.sprite == null)
                {
                    continue;
                }

                result.Add(new Result
                {
                    path = AssetDatabase.GetAssetPath(parent),
                    component = component
                });
            }
        }
    }

    [InitializeOnLoad]
    class RegisterRemoveDependencyRule
    {
        static RegisterRemoveDependencyRule()
        {
            AnalyzeSystem.RegisterNewRule<RemoveDependencyRule>();
        }
    }
}
