using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Elysia.Tables
{
    public readonly ref partial struct Field
    {
        public static implicit operator bool(Field field) => field._str.IsEmpty ? default : bool.Parse(field._str);
        public static implicit operator byte(Field field) => field._str.IsEmpty ? default : byte.Parse(field._str);
        public static implicit operator sbyte(Field field) => field._str.IsEmpty ? default : sbyte.Parse(field._str);
        public static implicit operator char(Field field) => field._str.IsEmpty ? default : field._str[0];
        public static implicit operator decimal(Field field) => field._str.IsEmpty ? default : decimal.Parse(field._str, provider: NumberFormatInfo.InvariantInfo);
        public static implicit operator double(Field field) => field._str.IsEmpty ? default : double.Parse(field._str, provider: NumberFormatInfo.InvariantInfo);
        public static implicit operator float(Field field) => field._str.IsEmpty ? default : float.Parse(field._str, provider: NumberFormatInfo.InvariantInfo);
        public static implicit operator int(Field field) => field._str.IsEmpty ? default : int.Parse(field._str, provider: NumberFormatInfo.InvariantInfo);
        public static implicit operator uint(Field field) => field._str.IsEmpty ? default : uint.Parse(field._str, provider: NumberFormatInfo.InvariantInfo);
        public static implicit operator long(Field field) => field._str.IsEmpty ? default : long.Parse(field._str, provider: NumberFormatInfo.InvariantInfo);
        public static implicit operator ulong(Field field) => field._str.IsEmpty ? default : ulong.Parse(field._str, provider: NumberFormatInfo.InvariantInfo);
        public static implicit operator short(Field field) => field._str.IsEmpty ? default : short.Parse(field._str, provider: NumberFormatInfo.InvariantInfo);
        public static implicit operator ushort(Field field) => field._str.IsEmpty ? default : ushort.Parse(field._str, provider: NumberFormatInfo.InvariantInfo);

        public static implicit operator string(Field field) => field._str.ToString();

        public static implicit operator Color(Field field)
        {
            int pos = 0;
            float r = field.Read(ref pos);
            float g = field.Read(ref pos);
            float b = field.Read(ref pos);
            float a = field.Read(ref pos);

            return new Color(r, g, b, a);
        }

        public static implicit operator Color32(Field field)
        {
            int pos = 0;
            byte r = field.Read(ref pos);
            byte g = field.Read(ref pos);
            byte b = field.Read(ref pos);
            byte a = field.Read(ref pos);

            return new Color32(r, g, b, a);
        }

        public static implicit operator Vector3(Field field)
        {
            int pos = 0;
            float x = field.Read(ref pos);
            float y = field.Read(ref pos);
            float z = field.Read(ref pos);

            return new Vector3(x, y, z);
        }

        public static implicit operator Vector2(Field field)
        {
            int pos = 0;
            float x = field.Read(ref pos);
            float y = field.Read(ref pos);

            return new Vector2(x, y);
        }

        public static implicit operator StringID(Field field) => field._str.IsEmpty ? StringID.NULL : new StringID(field._str.ToString());

        private readonly System.ReadOnlySpan<char> _str;

        public Field(System.ReadOnlySpan<char> str)
        {
            _str = str;
        }

        public T ToEnum<T>()
            where T : System.Enum
        {
            return _str.IsEmpty ? default : (T)System.Enum.Parse(typeof(T), _str.ToString(), false);
        }

        public delegate T Converter<out T>(Field field);

        public IReadOnlyList<T> ToArray<T>(Converter<T> converter)
        {
            T[] array = new T[GetElementCount()];
            int pos = 0;
            for (int i = 0; i < array.Length; i++)
            {
                Field element = GetElement(ref pos);
                array[i] = converter(element);
            }

            return array;
        }

        private int GetElementCount()
        {
            int count = 0;
            bool flag = false;
            for (int i = 0; i < _str.Length; i++)
            {
                switch (_str[i])
                {
                    case '[':
                        Debug.Assert(!flag);
                        flag = true;
                        break;

                    case ']':
                        Debug.Assert(flag);
                        flag = false;

                        count++;
                        break;
                }
            }

            Debug.Assert(flag == false);
            return count;
        }

        private Field GetElement(ref int pos)
        {
            while (_str[pos] != '[')
            {
                pos++;
                if (pos >= _str.Length)
                {
                    return default;
                }
            }

            pos++;
            int p0 = pos;

            while (_str[pos] != ']')
            {
                pos++;
                if (pos >= _str.Length)
                {
                    return default;
                }
            }

            int p1 = pos;

            return new Field(_str[p0..p1]);
        }

        private Field Read(ref int pos)
        {
            for (; pos < _str.Length; pos++)
            {
                if (_str[pos] != ' ' && _str[pos] != ',')
                {
                    break;
                }
            }

            int start = pos;
            for (; pos < _str.Length; pos++)
            {
                if (_str[pos] == ',')
                {
                    break;
                }
            }

            return new Field(_str[start..pos]);
        }
    }
}
