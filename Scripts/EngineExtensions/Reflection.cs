using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Elysia
{
    public static class Reflection
    {
        public const BindingFlags STATIC_FLAGS = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        public const BindingFlags INSTANCE_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static void CallMethod<T>(string methodName, params object[] parameters)
        {
            CallMethod<T>(typeof(T), methodName, parameters);
        }

        public static void CallMethod(Type type, string methodName, params object[] parameters)
        {
            CallMethod<object>(type, methodName, parameters);
        }

        public static TReturn CallMethod<T, TReturn>(string methodName, params object[] parameters)
        {
            return CallMethod<TReturn>(typeof(T), methodName, parameters);
        }

        public static TReturn CallMethod<TReturn>(Type type, string methodName, params object[] parameters)
        {
            Debug.Assert(type != null);
            Debug.Assert(!string.IsNullOrEmpty(methodName));

            MethodInfo methodInfo = FindMethod(type, methodName, STATIC_FLAGS);
            Debug.Assert(methodInfo != null, $"Static Method '{methodName}' not found in type '{type.FullName}'.");

            return (TReturn)methodInfo.Invoke(null, parameters);
        }

        public static void CallMethod(object target, string methodName, params object[] parameters)
        {
            CallMethod<object>(target, methodName, parameters);
        }

        public static TReturn CallMethod<TReturn>(object target, string methodName, params object[] parameters)
        {
            Debug.Assert(target != null);
            Debug.Assert(!string.IsNullOrEmpty(methodName));

            MethodInfo methodInfo = FindMethod(target.GetType(), methodName, INSTANCE_FLAGS);
            Debug.Assert(methodInfo != null, $"Method '{methodName}' not found in type '{target.GetType().FullName}'.");

            return (TReturn)methodInfo.Invoke(target, parameters);
        }

        public static TReturn GetField<T, TReturn>(string fieldName)
        {
            return GetField<TReturn>(typeof(T), fieldName);
        }

        public static TReturn GetField<TReturn>(Type type, string fieldName)
        {
            Debug.Assert(type != null);
            Debug.Assert(!string.IsNullOrEmpty(fieldName));

            FieldInfo field = FindField(type, fieldName, STATIC_FLAGS);
            Debug.Assert(field != null, $"Static Field '{fieldName}' not found in type '{type.FullName}'.");

            return (TReturn)field.GetValue(null);
        }

        public static TReturn GetField<TReturn>(object target, string fieldName)
        {
            Debug.Assert(target != null);
            Debug.Assert(!string.IsNullOrEmpty(fieldName));

            FieldInfo field = FindField(target.GetType(), fieldName, INSTANCE_FLAGS);
            Debug.Assert(field != null, $"Field '{fieldName}' not found in type '{target.GetType().FullName}'.");

            return (TReturn)field.GetValue(target);
        }

        public static void SetField<T>(string fieldName, object value)
        {
            SetField(typeof(T), fieldName, value);
        }

        public static void SetField(Type type, string fieldName, object value)
        {
            Debug.Assert(type != null);
            Debug.Assert(!string.IsNullOrEmpty(fieldName));

            FieldInfo field = FindField(type, fieldName, STATIC_FLAGS);
            Debug.Assert(field != null, $"Static Field '{fieldName}' not found in type '{type.FullName}'.");

            field.SetValue(null, value);
        }

        public static void SetField(object target, string fieldName, object value)
        {
            Debug.Assert(target != null);
            Debug.Assert(!string.IsNullOrEmpty(fieldName));

            FieldInfo field = FindField(target.GetType(), fieldName, INSTANCE_FLAGS);
            Debug.Assert(field != null, $"Field '{fieldName}' not found in type '{target.GetType().FullName}'.");

            field.SetValue(target, value);
        }

        public static TReturn GetProperty<T, TReturn>(string propertyName)
        {
            return GetProperty<TReturn>(typeof(T), propertyName);
        }

        public static TReturn GetProperty<TReturn>(Type type, string propertyName)
        {
            Debug.Assert(type != null);
            Debug.Assert(!string.IsNullOrEmpty(propertyName));

            PropertyInfo property = FindProperty(type, propertyName, STATIC_FLAGS);
            Debug.Assert(property != null, $"Static Property '{propertyName}' not found in type '{type.FullName}'.");

            return (TReturn)property.GetValue(null);
        }

        public static TReturn GetProperty<TReturn>(object target, string propertyName)
        {
            Debug.Assert(target != null);
            Debug.Assert(!string.IsNullOrEmpty(propertyName));

            PropertyInfo property = FindProperty(target.GetType(), propertyName, INSTANCE_FLAGS);
            Debug.Assert(property != null, $"Property '{propertyName}' not found in type '{target.GetType().FullName}'.");

            return (TReturn)property.GetValue(target);
        }

        public static void SetProperty<T>(string propertyName, object value)
        {
            SetProperty(typeof(T), propertyName, value);
        }

        public static void SetProperty(Type type, string propertyName, object value)
        {
            Debug.Assert(type != null);
            Debug.Assert(!string.IsNullOrEmpty(propertyName));

            PropertyInfo property = FindProperty(type, propertyName, STATIC_FLAGS);
            Debug.Assert(property != null, $"Static Property '{propertyName}' not found in type '{type.FullName}'.");

            property.SetValue(null, value);
        }

        public static void SetProperty(object target, string propertyName, object value)
        {
            Debug.Assert(target != null);
            Debug.Assert(!string.IsNullOrEmpty(propertyName));

            PropertyInfo property = FindProperty(target.GetType(), propertyName, INSTANCE_FLAGS);
            Debug.Assert(property != null, $"Property '{propertyName}' not found in type '{target.GetType().FullName}'.");

            property.SetValue(target, value);
        }

        public static MethodInfo FindMethod(Type type, string methodName, BindingFlags flags)
        {
            Debug.Assert(type != null);
            Debug.Assert(!string.IsNullOrEmpty(methodName));

            for (; type != null; type = type.BaseType)
            {
                MethodInfo method = type.GetMethod(methodName, flags);
                if (method != null)
                {
                    return method;
                }
            }

            return null;
        }

        public static FieldInfo FindField(Type type, string fieldName, BindingFlags flags)
        {
            Debug.Assert(type != null);
            Debug.Assert(!string.IsNullOrEmpty(fieldName));

            for (; type != null; type = type.BaseType)
            {
                FieldInfo field = type.GetField(fieldName, flags);
                if (field != null)
                {
                    return field;
                }
            }

            return null;
        }

        public static PropertyInfo FindProperty(Type type, string propertyName, BindingFlags flags)
        {
            Debug.Assert(type != null);
            Debug.Assert(!string.IsNullOrEmpty(propertyName));

            for (; type != null; type = type.BaseType)
            {
                PropertyInfo property = type.GetProperty(propertyName, flags);
                if (property != null)
                {
                    return property;
                }
            }

            return null;
        }
    }
}
