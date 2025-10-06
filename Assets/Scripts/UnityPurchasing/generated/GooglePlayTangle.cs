// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("9Ggai46+5ViVIJuqBt7g1BIO5tMXyD4A71wiP5CEJg/WRiF7tiRjZMZFS0R0xkVORsZFRUTLhOfGLtVnDGYW8WIESBSqfYgGcAKMUGHdWRMhR6wnXX8f/T9TUsXUh91Eot3vGin4Fe6SedcyJMFqDF6FUvsbWKEbG9/1cSvUmxZEKhXpfbsxImiS3iFsPR1goTeZqFuYwm2wDz28kR4u9wwdzN7emthmekX2H3B3s06KOC0TgSQIX2mzUI3l5xfrXzwgqsesORnNDCDXYYlYe4pz3QMMIErB/vdEGoQPx7qTbWYTKo2g3Akd6MDPcKUK/K2l0kU3mqlXmitgW5hkFC6c0Ox0xkVmdElCTW7CDMKzSUVFRUFERy+VXrAtG8YD30ZHRURF");
        private static int[] order = new int[] { 10,11,11,8,8,5,11,8,12,9,13,12,12,13,14 };
        private static int key = 68;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
