using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Elysia
{
    public struct ProfilerSampleScope : IDisposable
    {
        public ProfilerSampleScope(string name)
        {
            Profiler.BeginSample(name);
        }

        public ProfilerSampleScope(string name, UnityEngine.Object targetObject)
        {
            Profiler.BeginSample(name, targetObject);
        }

        public void Dispose()
        {
            Profiler.EndSample();
        }
    }
}
