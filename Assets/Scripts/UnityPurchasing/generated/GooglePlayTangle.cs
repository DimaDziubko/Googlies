// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Z0TqmFNcIPpuAMuy3Pyway8/V1exr8aEkmhavHxJ0HP2RWzd/RBLC5iZtYTGlENcm8ieGt6aSSxWIlXIJFvs63TNihnPICa55zlsjDZ1gKou2q4WI5n7ah6c5ceUw4/OnBE2shAhJgW8x9WlN17p0/jUCS4NRpI2Xt3T3Oxe3dbeXt3d3GvnCbMpZJX2gsvvQnbMZ8rXuA4x9Xg+lzHhzexe3f7s0drV9lqUWivR3d3d2dzfY6iSM8lrElwP5U98p7Zr5C3srHSIq4sSsoMyywSF/ZLgQZlwzdhXADF+HI0PchTn7vvgmASohBcIKfpfT7yZfWmr4DqI5O9K724glD6ASoizVpsyQ3oFbcsxckFbc8kEXuFfLWUHsUbNjFeXsd7f3dzd");
        private static int[] order = new int[] { 12,10,8,5,9,8,10,10,12,13,12,11,12,13,14 };
        private static int key = 220;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
