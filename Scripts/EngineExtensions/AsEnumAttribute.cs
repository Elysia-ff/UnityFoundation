using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AsEnumAttribute : PropertyAttribute
    {
        public readonly Type enumType;

        public AsEnumAttribute(Type enumType)
        {
            Debug.Assert(enumType.IsEnum);

            this.enumType = enumType;
        }
    }
}
