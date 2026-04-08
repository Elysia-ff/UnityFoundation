using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace Elysia
{
    public static class DataFiles
    {
        public static string RootPath { get; private set; }

        private static readonly Dictionary<Type, FileMeta> _fileMetas = new Dictionary<Type, FileMeta>();

        private static readonly JsonSerializer _serializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc
        });

        public static void Initialize(string uid)
        {
            RootPath = Path.Combine(Application.persistentDataPath, uid);
            _fileMetas.Clear();

            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
            }

#if UNITY_EDITOR
            MigrationHelper.Validate();
#endif
        }

        public static bool Save<T>(T data)
            where T : SaveData
        {
            FileMeta meta = GetFileMeta<T>();
#pragma warning disable CS0618
            data.Version = meta.Version;
#pragma warning restore CS0618

            try
            {
                {
                    using FileStream fileStream = new FileStream(meta.TempPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    using StreamWriter streamWriter = new StreamWriter(fileStream);
                    using JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter);

                    _serializer.Serialize(jsonWriter, data);
                }

                if (File.Exists(meta.FullPath))
                {
                    File.Delete(meta.BackUpPath);

                    File.Move(meta.FullPath, meta.BackUpPath);
                    File.Move(meta.TempPath, meta.FullPath);
                    File.Delete(meta.BackUpPath);
                }
                else
                {
                    File.Move(meta.TempPath, meta.FullPath);
                }

                Debug.Log($"[{nameof(DataFiles)}] Data saved: '{meta.FileName}'\n<a href=\"file:///{Path.GetDirectoryName(meta.FullPath)}\">{meta.FullPath}</a>\n");

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(DataFiles)}] Save failed: '{meta.FileName}'\n\n{e}");
                RecoverIfNeeded<T>();

                return false;
            }
        }

        public static bool Save<T>(T data, StringBuilder outJson)
            where T : SaveData
        {
            FileMeta meta = GetFileMeta<T>();
#pragma warning disable CS0618
            data.Version = meta.Version;
#pragma warning restore CS0618

            try
            {
                {
                    using FileStream fileStream = new FileStream(meta.TempPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    using StreamWriter streamWriter = new StreamWriter(fileStream);

                    using StringWriter stringWriter = new StringWriter(outJson);
                    using MultiTextWriter writer = new MultiTextWriter(streamWriter, stringWriter);

                    using JsonTextWriter jsonWriter = new JsonTextWriter(writer);

                    _serializer.Serialize(jsonWriter, data);
                }

                if (File.Exists(meta.FullPath))
                {
                    File.Delete(meta.BackUpPath);

                    File.Move(meta.FullPath, meta.BackUpPath);
                    File.Move(meta.TempPath, meta.FullPath);
                    File.Delete(meta.BackUpPath);
                }
                else
                {
                    File.Move(meta.TempPath, meta.FullPath);
                }

                Debug.Log($"[{nameof(DataFiles)}] Data saved: '{meta.FileName}'\n<a href=\"file:///{Path.GetDirectoryName(meta.FullPath)}\">{meta.FullPath}</a>\n");

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(DataFiles)}] Save failed: '{meta.FileName}'\n\n{e}");
                RecoverIfNeeded<T>();

                return false;
            }
        }

        public static bool SaveAsJson<T>(T data, StringBuilder outJson)
            where T : SaveData
        {
            FileMeta meta = GetFileMeta<T>();
#pragma warning disable CS0618
            data.Version = meta.Version;
#pragma warning restore CS0618

            try
            {
                {
                    using StringWriter stringWriter = new StringWriter(outJson);
                    using JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter);

                    _serializer.Serialize(jsonWriter, data);
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(DataFiles)}] Save failed: '{meta.FileName}'\n\n{e}");
                RecoverIfNeeded<T>();

                return false;
            }
        }

        public static T Load<T>()
            where T : SaveData
        {
            RecoverIfNeeded<T>();

            FileMeta meta = GetFileMeta<T>();

            try
            {
                T data = null;

                int version = ReadVersionFromFile(meta.FullPath);
                if (version < meta.Version)
                {
                    try
                    {
                        Type oldType = MigrationHelper.FindType(meta.FileName, version);
                        object temp = LoadFromFile(meta.FullPath, oldType);

                        for (int nextVersion = version + 1; nextVersion <= meta.Version;)
                        {
                            Type nextType = MigrationHelper.FindType(meta.FileName, nextVersion);
                            temp = MigrationHelper.InvokeMigrate(temp, oldType);
                            Debug.Assert(temp.GetType() == nextType);

                            oldType = nextType;
                            nextVersion++;
                        }

                        data = (T)temp;
#pragma warning disable CS0618
                        data.Version = meta.Version;
#pragma warning restore CS0618

                        Debug.Log($"[{nameof(DataFiles)}] Migrated: '{meta.FileName}'({version} -> {meta.Version})");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[{nameof(DataFiles)}] Migration failed: '{meta.FileName}({version})'\n\n{e}");
                    }
                }
                else
                {
                    data = (T)LoadFromFile(meta.FullPath, typeof(T));
                }

                return data;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(DataFiles)}] Load failed: '{meta.FileName}'\n\n{e}");
                return null;
            }
        }

        public static T Load<T>(string json)
            where T : SaveData
        {
            FileMeta meta = GetFileMeta<T>();

            try
            {
                T data = null;

                int version = ReadVersionFromJson(json);
                if (version < meta.Version)
                {
                    try
                    {
                        Type oldType = MigrationHelper.FindType(meta.FileName, version);
                        object temp = LoadFromJson(json, oldType);

                        for (int nextVersion = version + 1; nextVersion <= meta.Version;)
                        {
                            Type nextType = MigrationHelper.FindType(meta.FileName, nextVersion);
                            temp = MigrationHelper.InvokeMigrate(temp, oldType);
                            Debug.Assert(temp.GetType() == nextType);

                            oldType = nextType;
                            nextVersion++;
                        }

                        data = (T)temp;
#pragma warning disable CS0618
                        data.Version = meta.Version;
#pragma warning restore CS0618

                        Debug.Log($"[{nameof(DataFiles)}] Migrated: '{meta.FileName}'({version} -> {meta.Version})");
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"[{nameof(DataFiles)}] Migration failed: '{meta.FileName}({version})'\n\n{e}");
                    }
                }
                else
                {
                    data = (T)LoadFromJson(json, typeof(T));
                }

                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(DataFiles)}] Load failed: '{meta.FileName}'\n\n{e}");
                return null;
            }
        }

        private static void RecoverIfNeeded<T>()
            where T : SaveData
        {
            FileMeta meta = GetFileMeta<T>();

            try
            {
                if (File.Exists(meta.BackUpPath))
                {
                    // 원본이 남아있으면 백업 파일 정리
                    if (File.Exists(meta.FullPath))
                    {
                        File.Delete(meta.BackUpPath);

                        Debug.Log($"[{nameof(DataFiles)}] Recovery: '{meta.FileName}'\nCleaned up backup");
                    }
                    // 원본이 소실된 경우 백업 파일로 복구
                    else
                    {
                        File.Move(meta.BackUpPath, meta.FullPath);

                        Debug.Log($"[{nameof(DataFiles)}] Recovery: '{meta.FileName}'\nRolled back to backup");
                    }
                }

                if (File.Exists(meta.TempPath))
                {
                    // 유효한 최신 파일이 있는 경우 교체
                    if (IsValidFile(meta.TempPath))
                    {
                        File.Delete(meta.FullPath);
                        File.Move(meta.TempPath, meta.FullPath);

                        Debug.Log($"[{nameof(DataFiles)}] Recovery: '{meta.FileName}'\nApplied valid temp file");
                    }
                    // 손상된 파일 삭제
                    else
                    {
                        File.Delete(meta.TempPath);

                        Debug.Log($"[{nameof(DataFiles)}] Recovery: '{meta.FileName}'\nDeleted corrupted temp file");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[{nameof(DataFiles)}] Recovery failed: '{meta.FileName}'\n\n{e}");
            }
        }

        private static int ReadVersionFromFile(string path)
        {
            using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using StreamReader streamReader = new StreamReader(fileStream);
            using JsonTextReader jsonReader = new JsonTextReader(streamReader);

            return ReadVersion(jsonReader);
        }

        private static int ReadVersionFromJson(string json)
        {
            using StringReader stringReader = new StringReader(json);
            using JsonTextReader jsonReader = new JsonTextReader(stringReader);

            return ReadVersion(jsonReader);
        }

        private static int ReadVersion(JsonReader reader)
        {
            reader.Read();
            Debug.Assert(reader.TokenType == JsonToken.StartObject);

            reader.Read();
            if (reader.TokenType != JsonToken.PropertyName || (string)reader.Value != "Version")
            {
                throw new JsonNotFoundVersionException();
            }

            reader.Read();
            return Convert.ToInt32(reader.Value);
        }

        private static bool IsValidFile(string path)
        {
            try
            {
                using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                using StreamReader streamReader = new StreamReader(fileStream);
                using JsonTextReader jsonReader = new JsonTextReader(streamReader);

                return IsValid(jsonReader);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsValidJson(string json)
        {
            try
            {
                using StringReader stringReader = new StringReader(json);
                using JsonTextReader jsonReader = new JsonTextReader(stringReader);

                return IsValid(jsonReader);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValid(JsonReader jsonReader)
        {
            jsonReader.Read();
            if (jsonReader.TokenType != JsonToken.StartObject)
            {
                return false;
            }

            jsonReader.Read();
            if (jsonReader.TokenType != JsonToken.PropertyName || (string)jsonReader.Value != "Version")
            {
                return false;
            }

            jsonReader.Read();
            if (jsonReader.TokenType != JsonToken.Integer)
            {
                return false;
            }

            while (jsonReader.Read())
            {
            }

            return jsonReader.Depth == 0;
        }

        public static void Delete<T>()
            where T : SaveData
        {
            FileMeta meta = GetFileMeta<T>();

            try
            {
                File.Delete(meta.FullPath);
            }
            catch
            {
                // swallow exception
            }

            try
            {
                File.Delete(meta.TempPath);
            }
            catch
            {
                // swallow exception
            }

            try
            {
                File.Delete(meta.BackUpPath);
            }
            catch
            {
                // swallow exception
            }
        }

        private static object LoadFromFile(string path, Type type)
        {
            using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using StreamReader streamReader = new StreamReader(fileStream);
            using JsonTextReader jsonReader = new JsonTextReader(streamReader);

            return Load(jsonReader, type);
        }

        private static object LoadFromJson(string json, Type type)
        {
            using StringReader stringReader = new StringReader(json);
            using JsonTextReader jsonReader = new JsonTextReader(stringReader);

            return Load(jsonReader, type);
        }

        private static object Load(JsonReader jsonReader, Type type)
        {
            object obj = _serializer.Deserialize(jsonReader, type);
            if (obj == null)
            {
                throw new JsonEmptyException();
            }

            return obj;
        }

        public static FileMeta GetFileMeta<T>()
            where T : SaveData
        {
            Type type = typeof(T);
            if (!_fileMetas.TryGetValue(type, out FileMeta meta))
            {
                SaveDataAttribute attr = System.Reflection.CustomAttributeExtensions.GetCustomAttribute<SaveDataAttribute>(type, true);
                meta = new FileMeta(RootPath, attr.FileName, attr.Version);

                _fileMetas.Add(type, meta);
            }

            return meta;
        }
    }
}
