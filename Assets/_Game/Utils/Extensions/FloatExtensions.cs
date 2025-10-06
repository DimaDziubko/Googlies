using System;
using System.Globalization;

namespace _Game.Utils.Extensions
{
    public static class FloatExtensions
    {
        private static readonly string[] Suffixes = {"k", "m", "b", "t"};
        private static readonly char[] Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        
        public static string ToCompactFormat(this double value, double threshold = 0, bool fourDigits = false) 
        {
            if (Math.Abs(value) < threshold)
                return value.ToString("0", CultureInfo.InvariantCulture);

            int suffixIndex = -1;

            while (Math.Abs(value) >= 1000)
            {
                value /= 1000;
                suffixIndex++;
            }

            string suffix = GetSuffix(suffixIndex);
            if (suffix == null)
                return "MAX";

            string formattedValue = fourDigits ? TrimToFourSignificantDigits(value) : TrimToThreeSignificantDigits(value);
            return $"{formattedValue}{suffix}";
        }
        
        public static string ToCompactFormat(this int value, int threshold = 0, bool fourDigits = false)
        {
            return ((double)value).ToCompactFormat(threshold, fourDigits);
        }

        public static string ToCompactFormat(this uint value, int threshold = 0, bool fourDigits = false)
        {
            return ((double)value).ToCompactFormat(threshold, fourDigits);
        }

        public static string ToCompactFormat(this float value, float threshold = 0, bool fourDigits = false)
        {
            return ((double)value).ToCompactFormat(threshold, fourDigits);
        }

        private static string GetSuffix(int index)
        {
            if (index < 0)
                return "";

            if (index < Suffixes.Length)
                return Suffixes[index];

            index -= Suffixes.Length;

            int firstLetterIndex = index / Alphabet.Length;
            int secondLetterIndex = index % Alphabet.Length;

            if (firstLetterIndex >= Alphabet.Length)
                return null;

            if (firstLetterIndex == 0)
                return $"{Alphabet[secondLetterIndex]}";

            return $"{Alphabet[firstLetterIndex - 1]}{Alphabet[secondLetterIndex]}";
        }

        private static string TrimToThreeSignificantDigits(double value)
        {
            if (Math.Abs(value) < 1 && Math.Abs(value) > 0.01)
                return value.ToString("0.##", CultureInfo.InvariantCulture);
            
            string formatted = value.ToString("G3", CultureInfo.InvariantCulture);
            if (formatted.Contains("."))
            {
                formatted = formatted.TrimEnd('0').TrimEnd('.');
            }
            return formatted;
        }
        
        private static string TrimToFourSignificantDigits(double value)
        {
            if (Math.Abs(value) < 1)
                return value.ToString("0.###", CultureInfo.InvariantCulture);
            
            string formatted = value.ToString("G4", CultureInfo.InvariantCulture);
            if (formatted.Contains("."))
            {
                formatted = formatted.TrimEnd('0').TrimEnd('.');
            }
            return formatted;
        }
        
        public static bool ApproximatelyEqual(this float value1, float value2, float tolerance = float.Epsilon)
        {
            return Math.Abs(value1 - value2) < tolerance;
        }
    }
}