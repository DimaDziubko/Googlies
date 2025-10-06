using UnityEngine;

namespace _Game.Scenes.Tests
{
    public class Projectile_1 : MonoBehaviour
    {
        public Transform target; // Ціль, в яку має влучити снаряд
        public float speed = 10f; // Швидкість снаряда
        public Transform curvePoint; // Додаткова точка, що визначає криволінійність траєкторії

        private float startTime;
        private Vector3 startPosition;

        void Start()
        {
            startTime = Time.time;
            startPosition = transform.position;
        }

        void Update()
        {
            if(target == null) return;

            float t = (Time.time - startTime) * speed;
            Vector3 p0 = startPosition;
            Vector3 p1 = curvePoint.position;
            Vector3 p2 = target.position;

            // Розрахунок позиції снаряда на кривій Безьє
            Vector3 position = CalculateQuadraticBezierPoint(t, p0, p1, p2);
            transform.position = position;

            // Перевірка, чи снаряд дістався до цілі
            if(Vector3.Distance(transform.position, target.position) < 0.1f)
            {
                // Снаряд влучив в ціль
                Destroy(gameObject);
            }
        }

        Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;

            Vector3 p = uu * p0; // Терм для p0
            p += 2 * u * t * p1; // Терм для p1
            p += tt * p2; // Терм для p2

            return p;
        }
    
        private void CalculateInitialVelocityAndCurvePoint()
        {
            var startPosition = transform.position; // Початкова позиція снаряда
            var targetPosition = target.position; // Позиція цілі
    
            // Визначення середньої точки по X
            float midX = (startPosition.x + targetPosition.x) / 2;
    
            // Визначення "висоти" вигину. Це може бути фіксоване значення або змінна, що залежить від розстояння між ціллю і снарядом
            float height = Mathf.Abs(targetPosition.x - startPosition.x) / 4; // Наприклад, чверть відстані між снарядом і ціллю
    
            // Якщо потрібно, щоб вигин був завжди вверх, навіть якщо ціль нижче за снаряд, використовуйте Mathf.Max() для визначення максимального значення Y
            float maxY = Mathf.Max(startPosition.y, targetPosition.y) + height;
    
            // Створення вектора для контрольної точки
            Vector3 curvePoint = new Vector3(midX, maxY, 0);
    
            // Тепер, маючи curvePoint, можна розрахувати ініційовану швидкість або використовувати curvePoint безпосередньо при розрахунку траєкторії руху снаряда
        }
    }
}
