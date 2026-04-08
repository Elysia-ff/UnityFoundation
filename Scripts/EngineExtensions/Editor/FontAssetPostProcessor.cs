using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor;

namespace Elysia
{
    public class FontAssetPostProcessor : AssetModificationProcessor
    {
        private static readonly string ID_CLEAR_DYNAMIC_DATA = "FontAsset.ClearDynamicData";

        public static bool GetClearDynamicData()
        {
            return EditorPrefs.GetBool(ID_CLEAR_DYNAMIC_DATA, true);
        }

        public static void SetClearDynamicData(bool value)
        {
            EditorPrefs.SetBool(ID_CLEAR_DYNAMIC_DATA, value);
        }

        public static void Process(TMP_FontAsset fontAsset)
        {
            // if (fontAsset != null && fontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic && fontAsset.clearDynamicDataOnBuild && fontAsset.atlasTexture.width != 0)
            if (fontAsset != null && fontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic && fontAsset.atlasTexture.width != 0)
            {
                //Debug.Log($"[{nameof(FontAssetPostProcessor)}] Clearing '{fontAsset.name}' dynamic font asset data.");

                fontAsset.ClearFontAssetData(true);

                string path = AssetDatabase.GetAssetPath(fontAsset);
                Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (Object subAsset in subAssets)
                {
                    if (subAsset is not Texture2D || subAsset == fontAsset.atlasTexture || !subAsset.name.StartsWith(fontAsset.atlasTexture.name))
                    {
                        continue;
                    }

                    Object.DestroyImmediate(subAsset, true);
                }

                TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(true, fontAsset);
            }
        }

        private static string[] OnWillSaveAssets(string[] paths)
        {
            if (!GetClearDynamicData())
            {
                return paths;
            }

            for (int i = 0; i < paths.Length; i++)
            {
                try
                {
                    string path = paths[i];
                    if (!path.EndsWith(".asset"))
                    {
                        continue;
                    }

                    if (AssetDatabase.GetMainAssetTypeAtPath(path) != typeof(TMP_FontAsset))
                    {
                        continue;
                    }

                    TMP_FontAsset fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
                    Process(fontAsset);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e);
                }
            }

            return paths;
        }
    }
}
