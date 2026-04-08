using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia.Tables
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EqualOperatorAttribute : Attribute
    {
        public string TypeName { get; }

        public EqualOperatorAttribute(string typeName)
        {
            TypeName = typeName;
        }
    }
}
