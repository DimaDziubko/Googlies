using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace _Game.Utils
{
    public static class PerfMethodTimer
    {
        public static void Measure(string label, Action action)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            Debug.Log($"[PerfTimer] {label}: {sw.ElapsedMilliseconds} ms");
        }
        
        public static T Measure<T>(string label, Func<T> func)
        {
            var sw = Stopwatch.StartNew();
            T result = func();
            sw.Stop();
            Debug.Log($"[PerfTimer] {label}: {sw.ElapsedMilliseconds} ms");
            return result;
        }
    }
}