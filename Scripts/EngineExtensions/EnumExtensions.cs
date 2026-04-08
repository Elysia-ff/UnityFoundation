using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    public static class EnumExtensions
    {
        public unsafe static T ToInt<TEnum, T>(this TEnum enumValue)
            where TEnum : unmanaged, Enum
            where T : unmanaged
        {
            Debug.Assert(sizeof(TEnum) == sizeof(T));

            TEnum* ptr = &enumValue;
            T* intValue = (T*)ptr;

            return *intValue;
        }

        public unsafe static TEnum ToEnum<T, TEnum>(this T intValue)
            where T : unmanaged
            where TEnum : unmanaged, Enum
        {
            Debug.Assert(sizeof(T) == sizeof(TEnum));

            T* ptr = &intValue;
            TEnum* enumValue = (TEnum*)ptr;

            return *enumValue;
        }
    }
}
