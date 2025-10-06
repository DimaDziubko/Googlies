using System;

namespace _Game.Utils.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string ToCondensedTimeFormat(this TimeSpan timeSpan)
        {
            if (timeSpan.Days > 0)
            {
                return $"{timeSpan.Days}d {timeSpan.Hours}h";
            }
            
            if (timeSpan.Hours > 0)
            {
                return $"{timeSpan.Hours}h {timeSpan.Minutes}m";
            }

            if (timeSpan.Minutes > 0)
            {
                return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
            }
            
            return $"{timeSpan.Seconds}s";
        }
        public static int ToCompactInt(this DateTime dateTime)
        {
            return int.Parse(dateTime.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}