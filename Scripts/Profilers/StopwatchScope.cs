using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Elysia
{
    public readonly struct StopwatchScope : IDisposable
    {
        private static readonly Stopwatch SW = new Stopwatch();

        private readonly Stopwatch _stopwatch;
        private readonly string _format;

        public StopwatchScope(Stopwatch stopwatch, string format)
        {
            Debug.Assert(!stopwatch.IsRunning);

            _stopwatch = stopwatch;
            _format = format;

            _stopwatch.Restart();
        }

        public StopwatchScope(string format)
        {
            Debug.Assert(!SW.IsRunning);

            _stopwatch = SW;
            _format = format;

            _stopwatch.Restart();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            UnityEngine.Debug.LogWarning(string.Format(_format, _stopwatch.ElapsedMilliseconds));
        }
    }
}
