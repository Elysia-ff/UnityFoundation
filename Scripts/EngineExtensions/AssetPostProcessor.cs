#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace Elysia
{
    public class AssetPostProcessor : AssetPostprocessor
    {
        private readonly struct ProcessScope : IDisposable
        {
            public ProcessScope(bool value)
            {
                _shouldBeSaved = value;
                _assetsToBeSerialized.Clear();

                RefreshAddressableEntries();
            }

            public void Dispose()
            {
                IsRefreshing = true;

                try
                {
                    if (_shouldBeSaved)
                    {
                        AssetDatabase.ForceReserializeAssets(_assetsToBeSerialized);

                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(e);
                }
                finally
                {
                    IsRefreshing = false;
                    _shouldBeSaved = false;
                    _assetsToBeSerialized.Clear();
                }
            }
        }

        public static bool IsRefreshing { get; private set; }

        private static readonly HashSet<string> _candidates = new HashSet<string>();
        private static bool _shouldBeSaved;
        private static readonly List<string> _assetsToBeSerialized = new List<string>();

        public static readonly HashSet<string> FILE_EXTENSIONS = new HashSet<string>
        {
            ".asset", ".prefab", ".unity"
        };

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            _candidates.Clear();

            if (movedAssets.Length > 0)
            {
                RefreshAddressableEntries();
                CollectCandidates(movedAssets);
            }

            if (_candidates.Count > 0)
            {
                using var _ = new ProcessScope(false);
                foreach (string guid in _candidates)
                {
                    RefreshAssetUsingGuid(guid);
                }
            }
        }

        private static void CollectCandidates(string[] assets)
        {
            for (int i = 0; i < assets.Length; i++)
            {
                string guid = AssetDatabase.AssetPathToGUID(assets[i]);
                if (IsAddressableAsset(guid))
                {
                    _candidates.Add(guid);
                }
            }
        }

        public static string GetAddress(string guid)
        {
            if (string.IsNullOrEmpty(guid))
            {
                return string.Empty;
            }

            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetEntry entry = settings.FindAssetEntry(guid, true);

            return entry != null ? entry.address : string.Empty;
        }

        public static bool IsAddressableAsset(string guid)
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetEntry entry = settings.FindAssetEntry(guid, true);
            return entry != null;
        }

        private static void RefreshAddressableEntries()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            Reflection.SetField(settings, "m_FindAssetEntryCache", null);
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssetIfDirty(settings);
        }

        private static void RefreshAssetUsingGuid(string guid)
        {
            if (guid == null)
            {
                var assetPaths = AssetDatabase.GetAllAssetPaths().Where(IsProjectAsset);
                RefreshAssets(assetPaths);
            }
            else
            {
                ConcurrentQueue<string> assetQueue = new ConcurrentQueue<string>();
                List<Task> tasks = new List<Task>();

                byte[] pattern = System.Text.Encoding.UTF8.GetBytes(guid);

                string[] assetPaths = AssetDatabase.GetAllAssetPaths();
                foreach (string assetPath in assetPaths)
                {
                    if (!IsProjectAsset(assetPath))
                    {
                        continue;
                    }

                    Task task = Task.Run(async () =>
                    {
                        byte[] bytes = await System.IO.File.ReadAllBytesAsync(assetPath);
                        if (SearchPatterns(bytes, pattern))
                        {
                            assetQueue.Enqueue(assetPath);
                        }
                    });

                    tasks.Add(task);
                }

                Task.WhenAll(tasks).Wait();

                RefreshAssets(assetQueue);
            }
        }

        private static bool SearchPatterns(byte[] data, byte[] pattern)
        {
            const int TABLE_LENGTH = 256;
            int[] skipTable = new int[TABLE_LENGTH];
            {
                for (int i = 0; i < TABLE_LENGTH; i++)
                {
                    skipTable[i] = pattern.Length;
                }

                for (int i = 0; i < pattern.Length - 1; i++)
                {
                    skipTable[pattern[i]] = pattern.Length - 1 - i;
                }
            }

            int searchLength = data.Length;

            {
                int i = 0;
                while (i <= searchLength - pattern.Length)
                {
                    int j;
                    for (j = pattern.Length - 1; j >= 0; j--)
                    {
                        if (data[i + j] != pattern[j])
                        {
                            break;
                        }
                    }

                    if (j < 0)
                    {
                        return true;
                    }

                    i += skipTable[data[i + pattern.Length - 1]];
                }
            }

            return false;
        }

        private static void RefreshAssets(IEnumerable<string> assetPaths)
        {
            foreach (string assetPath in assetPaths)
            {
                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                UnityEngine.Debug.Log($"[{nameof(AssetPostProcessor)}] '{assetPath}' has been refreshed.", asset);

                _assetsToBeSerialized.Add(assetPath);
                // if (asset is SceneAsset)
                // {
                //     _assetsToBeSerialized.Add(assetPath);
                // }
                // else
                // {
                //     EditorUtility.SetDirty(asset);
                // }

                _shouldBeSaved = true;
            }
        }

        public static bool IsProjectAsset(string assetPath)
        {
            if (!assetPath.StartsWith("Assets/") || !FILE_EXTENSIONS.Contains(System.IO.Path.GetExtension(assetPath)))
            {
                return false;
            }

            Type t = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            if (t?.Namespace == null)
            {
                return false;
            }

            if (t != typeof(GameObject) && t != typeof(SceneAsset) && !t.Namespace.StartsWith("Elysia"))
            {
                return false;
            }

            return true;
        }

        [MenuItem("Assets/AssetPostProcessor/Is this Project Asset?", priority = 10000)]
        public static void PrintIsProjectAsset()
        {
            if (Selection.objects == null)
            {
                return;
            }

            for (int i = 0; i < Selection.objects.Length; i++)
            {
                string path = AssetDatabase.GetAssetPath(Selection.objects[i]);
                UnityEngine.Debug.Log($"[{nameof(AssetPostProcessor)}] {nameof(IsProjectAsset)}({path}) : {IsProjectAsset(path)}");
            }
        }

        [MenuItem("Assets/AssetPostProcessor/Refresh using This Asset", priority = 10001)]
        public static void RefreshUsingThisAsset()
        {
            if (Selection.objects == null)
            {
                return;
            }

            using var _ = new ProcessScope(false);
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                string path = AssetDatabase.GetAssetPath(Selection.objects[i]);
                string guid = AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets);
                if (string.IsNullOrEmpty(guid))
                {
                    continue;
                }

                RefreshAssetUsingGuid(guid);
            }
        }

        [MenuItem("Assets/AssetPostProcessor/Refresh This", priority = 10002)]
        public static void RefreshThis()
        {
            if (Selection.objects == null)
            {
                return;
            }

            using var _ = new ProcessScope(false);
            List<string> assetPaths = new List<string>(Selection.objects.Length);
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                string path = AssetDatabase.GetAssetPath(Selection.objects[i]);
                assetPaths.Add(path);
            }

            RefreshAssets(assetPaths);
        }

        [MenuItem("Assets/AssetPostProcessor/Refresh All", priority = 11000)]
        public static void RefreshAll()
        {
            using var _ = new ProcessScope(false);

            RefreshAssetUsingGuid(null);
        }
    }
}
#endif
