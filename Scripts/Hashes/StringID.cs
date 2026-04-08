using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Elysia
{
    [JsonConverter(typeof(StringIDJsonConverter))]
    public readonly struct StringID : IEquatable<StringID>, IKeyEvaluator
    {
        public static readonly StringID NULL = new StringID(default);

        public static explicit operator StringID(string value) => new StringID(value);
        public static explicit operator string(StringID id) => id._value;

        public static bool operator ==(StringID lhs, StringID rhs) => object.ReferenceEquals(lhs._value, rhs._value);
        public static bool operator !=(StringID lhs, StringID rhs) => !object.ReferenceEquals(lhs._value, rhs._value);

        public object RuntimeKey => _value;

        private readonly string _value;

        public StringID(string value)
        {
            _value = value != null ? string.Intern(value) : default;
        }

        public bool Equals(StringID other)
        {
            return object.ReferenceEquals(_value, other._value);
        }

        public override bool Equals(object obj)
        {
            return obj is StringID other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_value);
        }

        public override string ToString()
        {
            return _value;
        }

        public bool RuntimeKeyIsValid()
        {
            return !string.IsNullOrEmpty(_value);
        }
    }
}
