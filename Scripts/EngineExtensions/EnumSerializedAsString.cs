using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    [Serializable]
    public struct EnumSerializedAsString<T> : ISerializationCallbackReceiver
        where T : unmanaged, Enum
    {
        [SerializeField] private string _stringValue;
        public string StringValue => _stringValue;

        private T _enumValue;

        public static implicit operator T(EnumSerializedAsString<T> e)
        {
            return e._enumValue;
        }

        public EnumSerializedAsString(string stringValue)
        {
            _stringValue = stringValue;
            _enumValue = default;

            Parse();
        }

        public EnumSerializedAsString(T enumValue)
        {
            _stringValue = enumValue.ToString();
            _enumValue = enumValue;
        }

        public override string ToString()
        {
            Debug.Assert(Validate());

            return _enumValue.ToString();
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
            Parse();
        }

        private void Parse()
        {
            if (string.IsNullOrEmpty(_stringValue))
            {
                _enumValue = GetDefaultValue();
            }
            else
            {
#if UNITY_EDITOR
                try
                {
#endif
                    _enumValue = Enum.Parse<T>(_stringValue);
#if UNITY_EDITOR
                }
                catch
                {
                    UnityEngine.Debug.LogError($"{nameof(_stringValue)} Parse failed '{_stringValue}'");

                    throw;
                }
#endif
            }

            Debug.Assert(Validate());
        }

        private T GetDefaultValue()
        {
            Type type = typeof(T);
            if (!type.IsDefined(typeof(DefaultValueAttribute), true))
            {
                return default;
            }

            DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(DefaultValueAttribute));
            return defaultValueAttribute.value.ToEnum<int, T>();
        }

        private bool Validate()
        {
            if (string.IsNullOrEmpty(_stringValue) && _enumValue.Equals(GetDefaultValue()))
            {
                return true;
            }

            if (Enum.TryParse(_stringValue, out T assertionEnum) && assertionEnum.Equals(_enumValue))
            {
                return true;
            }

            return false;
        }
    }
}
