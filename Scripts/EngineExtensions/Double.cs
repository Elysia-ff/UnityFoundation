using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public readonly struct Double : IComparable<Double>
    {
        [Obsolete("Don't use float", true)]
        public static implicit operator Double(float _) => default;
        public static implicit operator Double(double time) => new Double(time);
        [Obsolete("Don't use float", true)]
        public static explicit operator float(Double _) => default;
        public static explicit operator double(Double time) => time._value;

        public static Double operator +(Double lhs, double rhs) => new Double(lhs._value + rhs);
        public static Double operator +(double lhs, Double rhs) => new Double(lhs + rhs._value);
        public static Double operator +(Double lhs, Double rhs) => new Double(lhs._value + rhs._value);

        public static Double operator -(Double lhs, double rhs) => new Double(lhs._value - rhs);
        public static Double operator -(double lhs, Double rhs) => new Double(lhs - rhs._value);
        public static Double operator -(Double lhs, Double rhs) => new Double(lhs._value - rhs._value);

        public static Double operator *(Double lhs, double rhs) => new Double(lhs._value * rhs);
        public static Double operator *(double lhs, Double rhs) => new Double(lhs * rhs._value);
        public static Double operator *(Double lhs, Double rhs) => new Double(lhs._value * rhs._value);

        public static Double operator /(Double lhs, double rhs) => new Double(lhs._value / rhs);
        public static Double operator /(double lhs, Double rhs) => new Double(lhs / rhs._value);
        public static Double operator /(Double lhs, Double rhs) => new Double(lhs._value / rhs._value);

        [Obsolete("Don't use float", true)]
        public static bool operator <(Double _, float __) => false;
        public static bool operator <(Double lhs, double rhs) => lhs._value < rhs;
        [Obsolete("Don't use float", true)]
        public static bool operator <(float _, Double __) => false;
        public static bool operator <(double lhs, Double rhs) => lhs < rhs._value;
        public static bool operator <(Double lhs, Double rhs) => lhs._value < rhs._value;

        [Obsolete("Don't use float", true)]
        public static bool operator >(Double _, float __) => false;
        public static bool operator >(Double lhs, double rhs) => lhs._value > rhs;
        [Obsolete("Don't use float", true)]
        public static bool operator >(float _, Double __) => false;
        public static bool operator >(double lhs, Double rhs) => lhs > rhs._value;
        public static bool operator >(Double lhs, Double rhs) => lhs._value > rhs._value;

        [Obsolete("Don't use float", true)]
        public static bool operator <=(Double _, float __) => false;
        public static bool operator <=(Double lhs, double rhs) => lhs._value <= rhs;
        [Obsolete("Don't use float", true)]
        public static bool operator <=(float _, Double __) => false;
        public static bool operator <=(double lhs, Double rhs) => lhs <= rhs._value;
        public static bool operator <=(Double lhs, Double rhs) => lhs._value <= rhs._value;

        [Obsolete("Don't use float", true)]
        public static bool operator >=(Double _, float __) => false;
        public static bool operator >=(Double lhs, double rhs) => lhs._value >= rhs;
        [Obsolete("Don't use float", true)]
        public static bool operator >=(float _, Double __) => false;
        public static bool operator >=(double lhs, Double rhs) => lhs >= rhs._value;
        public static bool operator >=(Double lhs, Double rhs) => lhs._value >= rhs._value;

        private readonly double _value;

        [Obsolete("Don't use float", true)]
        public Double(float value)
        {
            _value = value;
        }

        public Double(double value)
        {
            _value = value;
        }

        public int CompareTo(Double other)
        {
            return _value.CompareTo(other._value);
        }

        public override string ToString()
        {
            // ReSharper disable once SpecifyACultureInStringConversionExplicitly
            return _value.ToString();
        }
    }
}
