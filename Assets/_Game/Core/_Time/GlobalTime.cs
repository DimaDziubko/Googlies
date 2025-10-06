using System;

namespace _Game.Core._Time
{
    public static class GlobalTime
    {
        public static DateTime UtcNow => Instance.UtcNow;
        public static ITimeProvider Instance { get; internal set; }
    }
}