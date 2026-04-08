using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Elysia
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SaveDataAttribute : Attribute
    {
        public string FileName { get; }
        public int Version { get; }

        public SaveDataAttribute(string fileName, int version)
        {
            FileName = fileName;
            Version = version;
        }
    }
}
