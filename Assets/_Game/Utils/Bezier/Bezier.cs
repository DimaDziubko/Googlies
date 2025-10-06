using UnityEngine;

namespace _Game.Utils.Bezier
{
    public static class Bezier
    {
        public static Vector3 GetPoint(Vector3[] points, float t)
        {
            t = Mathf.Clamp01(t);
            int order = points.Length - 1;
            Vector3 result = Vector3.zero;

            for (int i = 0; i <= order; i++)
            {
                result += 
                    BinominalCoefficient(order, i) * Mathf.Pow(1 - t, order - i) * Mathf.Pow(t, i) * points[i];
            }

            return result;
        }
        
        public static Vector3 GetFirstDerivative(Vector3[] points, float t)
        {
            t = Mathf.Clamp01(t);
            int order = points.Length - 2; 
            Vector3 result = Vector3.zero;

            for (int i = 0; i <= order; i++)
            {
                Vector3 delta = points[i + 1] - points[i];
                result += BinominalCoefficient(order, i) * Mathf.Pow(1 - t, order - i) * Mathf.Pow(t, i) * delta * (order + 1);
            }

            return result;
        }

        private static float BinominalCoefficient(int n, in int k)
        {
            if (k < 0 || k > n) return 0;

            int result = 1;
            for (int i = 1; i <= k; ++i)
            {
                result *= n--;
                result /= i;
            }

            return result;
        }

        public static float CalculateCurveLength(Vector3[] points, int segments = 20)
        {
            float length = 0.0f;
            Vector3 previousPoint = GetPoint(points, 0);

            for (int i = 1; i <= segments; i++)
            {
                float t = i / (float) segments;
                Vector3 currentPoint = GetPoint(points, t);
                length += Vector3.Distance(previousPoint, currentPoint);
                previousPoint = currentPoint;
            }

            return length;
        }
    }
}