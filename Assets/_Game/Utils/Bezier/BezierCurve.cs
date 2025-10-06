using UnityEngine;

namespace Assets._Game.Utils.Bezier
{
    public class BezierCurve : MonoBehaviour
    {
        [SerializeField] private BezierPoint[] _points;
        
        public Vector3 GetPoint(float t)
        {
            if (_points == null || _points.Length < 2)
                return Vector3.zero;

            int segments = _points.Length - 1;
            int segment = Mathf.Min(Mathf.FloorToInt(t * segments), segments - 1);
            t = t * segments - segment;

            Vector3 p0 = _points[segment].MiddlePosition;
            Vector3 p1 = _points[segment].SecondHandlerPosition;
            Vector3 p2 = _points[segment + 1].FirstHandlerPosition;
            Vector3 p3 = _points[segment + 1].MiddlePosition;

            return global::_Game.Utils.Bezier.Bezier.GetPoint(new Vector3[] { p0, p1, p2, p3 }, t);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            DrawBezierCurve();
        }

        private void DrawBezierCurve()
        {
            // Кількість сегментів для наближення кривої
            int segments = 50;
            float step = 1f / segments;

            // Початкова точка кривої
            Vector3 startPoint = GetPoint(0f);

            for (int i = 1; i <= segments; i++)
            {
                // Визначення параметра t
                float t = i * step;

                // Отримання наступної точки кривої
                Vector3 endPoint = GetPoint(t);

                // Відображення лінії між двома точками
                Gizmos.DrawLine(startPoint, endPoint);

                // Оновлення початкової точки
                startPoint = endPoint;
            }
        }
#endif
    }
    
}