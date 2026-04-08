using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class DefaultValueAttribute : Attribute
    {
        public readonly int value = 0;

        public DefaultValueAttribute(int value)
        {
            this.value = value;
        }
    }
}
