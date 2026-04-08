using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Elysia
{
    public static class MigrationHelper
    {
        private static readonly Dictionary<(string fileName, int version), Type> _types;

        static MigrationHelper()
        {
            _types = Assembly.GetExecutingAssembly()
                             .GetTypes()
                             .Where(type => type.IsDefined(typeof(SaveDataAttribute)))
                             .ToDictionary(
                                 type =>
                                 {
                                     SaveDataAttribute attr = type.GetCustomAttribute<SaveDataAttribute>();
                                     return (attr.FileName, attr.Version);
                                 },
                                 type => type);
        }

        public static void Validate()
        {
            foreach (KeyValuePair<(string fileName, int version), Type> kv in _types)
            {
                if (!_types.TryGetValue((kv.Key.fileName, kv.Key.version + 1), out Type nextType))
                {
                    continue;
                }

                Type interfaceType = kv.Value.GetInterface(typeof(IMigrator<>).Name);
                if (interfaceType == null || interfaceType.GenericTypeArguments[0] != nextType)
                {
                    UnityEngine.Debug.LogError($"[{nameof(MigrationHelper)}] Migration interface not implemented\n'{kv.Value.FullName}' should have interface '{nameof(IMigrator<SaveData>)}<{nextType.FullName}>'");
                }
            }
        }

        public static Type FindType(string fileName, int version)
        {
            return _types[(fileName, version)];
        }

        public static object InvokeMigrate(object data, Type type)
        {
            MethodInfo method = type.GetInterface(typeof(IMigrator<>).Name).GetMethod(nameof(IMigrator<SaveData>.Migrate), Reflection.INSTANCE_FLAGS);
            return method!.Invoke(data, null);
        }
    }
}
