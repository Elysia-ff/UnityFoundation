using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elysia.Localizations;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.AnalyzeRules;
using UnityEditor.AddressableAssets.Settings;

namespace Elysia.AnalyzeRules
{
    public class CheckLocalizationComponentRule : AnalyzeRule
    {
        private enum EResultType
        {
            FontDataOverridden,
            FontPresetIsNull
        }

        private struct Result
        {
            public EResultType type;
            public string path;
            public TMP_Text component;
        }

        public override string ruleName => "Check Localization Components";

        public override bool CanFix => true;

        private List<Result> _results;

        private static readonly Dictionary<EResultType, string> ERROR_MSG = new Dictionary<EResultType, string>
        {
            { EResultType.FontDataOverridden, "Font Data has been overridden" },
            { EResultType.FontPresetIsNull, "Font Preset is null" }
        };

        public override void FixIssues(AddressableAssetSettings settings)
        {
            if (_results == null || _results.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _results.Count; i++)
            {
                Result result = _results[i];
                if (result.type == EResultType.FontDataOverridden)
                {
                    FixIssue(_results[i]);

                    GUID guid = AssetDatabase.GUIDFromAssetPath(_results[i].path);
                    AssetDatabase.SaveAssetIfDirty(guid);
                }
            }
        }

        private void FixIssue(Result result)
        {
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(result.path);
            if (obj == null)
            {
                return;
            }

            FontPreset.NullifyFontData(result.component);
            EditorUtility.SetDirty(obj);
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
                CollectInvalidLabel(obj, _results);
            }

            if (_results.Count == 0)
            {
                return new List<AnalyzeResult> { noErrors };
            }

            return _results.Select(e => new AnalyzeResult
            {
                resultName = $"{ERROR_MSG[e.type]}{kDelimiter}{e.path}",
                severity = MessageType.Warning
            }).ToList();
        }

        public override void ClearAnalysis()
        {
            base.ClearAnalysis();

            _results = null;
        }

        private void CollectInvalidLabel(GameObject parent, List<Result> result)
        {
            Label[] components = parent.GetComponentsInChildren<Label>(true);
            for (int i = 0; i < components.Length; i++)
            {
                Label component = components[i];
                LocalizedAsset<FontPreset> asset = Reflection.GetField<LocalizedAsset<FontPreset>>(component, "_fontPreset");
                if (asset.IsEmpty)
                {
                    result.Add(new Result
                    {
                        type = EResultType.FontPresetIsNull,
                        path = AssetDatabase.GetAssetPath(parent),
                        component = component
                    });
                }
                else if (!FontPreset.IsFontDataOverridden(component))
                {
                    result.Add(new Result
                    {
                        type = EResultType.FontDataOverridden,
                        path = AssetDatabase.GetAssetPath(parent),
                        component = component
                    });
                }
            }
        }
    }

    [InitializeOnLoad]
    class RegisterCheckLocalizationComponentRule
    {
        static RegisterCheckLocalizationComponentRule()
        {
            AnalyzeSystem.RegisterNewRule<CheckLocalizationComponentRule>();
        }
    }
}
