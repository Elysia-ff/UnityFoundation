using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Elysia
{
    public readonly struct CheckResizeScope : IDisposable
    {
        private readonly object _target;
        private readonly string _targetName;
        private readonly int _capacity;

        public CheckResizeScope(object target, string name)
        {
            _target = target;
            _targetName = name;
            _capacity = GetCapacity(_target);
        }

        public void Dispose()
        {
            int capacity = GetCapacity(_target);
            if (_capacity != capacity)
            {
                Debug.LogWarning($"[{nameof(CheckResizeScope)}] Re-allocation occured '{_targetName}' ({_capacity} -> {capacity})");
            }
        }

        public static void CheckWastedCapacity(object target, string targetName, int threshold)
        {
            int capacity = GetCapacity(target);
            int size = GetSize(target);

            if (capacity > threshold)
            {
                Debug.LogWarning($"[{nameof(CheckResizeScope)}] Capacity can be optimized '{targetName}' capacity: {capacity}, size: {size}, threshold: {threshold}");
            }
        }

        public static void CheckWastedCapacityPrime(object target, string targetName)
        {
            Debug.Assert(IsHashTable(target));

            int capacity = GetCapacity(target);
            int size = GetSize(target);

            Type hashHelperType = typeof(System.Collections.IDictionary).Assembly.GetType("System.Collections.HashHelpers");
            int expectedCapacity = Reflection.CallMethod<int>(hashHelperType, "GetPrime", size);

            if (capacity != expectedCapacity)
            {
                Debug.LogWarning($"[{nameof(CheckResizeScope)}] Capacity can be optimized '{targetName}' capacity: {capacity}, size: {size}, expected: {expectedCapacity}");
            }
        }

        private static int GetCapacity(object target)
        {
            Type type = target.GetType();

            // List
            PropertyInfo capacityProperty = Reflection.FindProperty(type, "Capacity", Reflection.INSTANCE_FLAGS);
            if (capacityProperty != null)
            {
                return (int)capacityProperty.GetValue(target);
            }

            // Dictionary, HashSet
            FieldInfo bucketField = Reflection.FindField(type, "_buckets", Reflection.INSTANCE_FLAGS);
            if (bucketField != null)
            {
                return bucketField.GetValue(target) is Array array ? array.Length : 0;
            }

            // Queue, Stack
            FieldInfo arrayField = Reflection.FindField(type, "_array", Reflection.INSTANCE_FLAGS);
            if (arrayField != null)
            {
                return arrayField.GetValue(target) is Array array ? array.Length : 0;
            }

            throw new NotSupportedException($"Unsupported type '{type}'");
        }

        private static int GetSize(object target)
        {
            Type type = target.GetType();

            PropertyInfo countProperty = Reflection.FindProperty(type, "Count", Reflection.INSTANCE_FLAGS);
            if (countProperty != null)
            {
                return (int)countProperty.GetValue(target);
            }

            throw new NotSupportedException($"Unsupported type '{type}'");
        }

        private static bool IsHashTable(object target)
        {
            // Dictionary, HashSet
            FieldInfo bucketField = Reflection.FindField(target.GetType(), "_buckets", Reflection.INSTANCE_FLAGS);

            return bucketField != null;
        }
    }
}
