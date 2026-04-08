using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Elysia
{
    public class FileMeta
    {
        public string FileName { get; }
        public string FullPath { get; }
        public string TempPath { get; }
        public string BackUpPath { get; }

        public int Version { get; }

        public FileMeta(string rootPath, string fileName, int version)
        {
            FileName = fileName;
            FullPath = Path.Combine(rootPath, $"{fileName}.save");
            TempPath = $"{FullPath}.tmp";
            BackUpPath = $"{FullPath}.bak";

            Version = version;
        }
    }
}
