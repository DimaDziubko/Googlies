using UnityEngine;

namespace _Game.Scenes.Tests
{
    public class Bullet : MonoBehaviour
    {
        public Transform MyTransform;

        private bool _isInited = false;

        private Vector3 initialVelocity;
        public Vector3 Position
        {
            get => MyTransform.position;
            set => MyTransform.position = value;
        }
    
        public Quaternion Rotation
        {
            get => MyTransform.rotation;
            set => MyTransform.rotation = value;
        }

        private Transform _target;
        public Transform Target
        {
            get => _target;
            set
            {
                _target = value;
                _isInited = true;
            }
        }
        public float FiringAngle = 45.0f;
        public float gravity = 9.8f;

        private float elapseTime;

        private void Start()
        {
            CalculateInitialVelocity();
            Destroy(this, 10);
        }

        void Update()
        {
            if (!_isInited) return;

            // Перевірка відстані до цілі
            float distanceToTarget = Vector3.Distance(Position, Target.position);
    
            if (distanceToTarget < 1.0f) // "1.0f" - це прикладна відстань коригування
            {
                // Визначаємо новий напрямок руху безпосередньо до цілі
                Vector3 directionToTarget = (Target.position - Position).normalized;
                // Коригування напрямку руху, зберігаючи постійну швидкість
                Position += directionToTarget * (initialVelocity.magnitude * Time.deltaTime);
            }
            else
            {
                // Рух кулі з урахуванням гравітації
                Position += initialVelocity * Time.deltaTime;
                initialVelocity.y -= gravity * Time.deltaTime;
            }

            // Оновлення орієнтації кулі в напрямку руху
            if (initialVelocity != Vector3.zero) {
                Rotation = Quaternion.LookRotation(initialVelocity);
            }
        }

        private void CalculateInitialVelocity()
        {
            var targetPosition = Target.position;
            float targetDistance = Vector3.Distance(Position, targetPosition);
            float projectileVelocity = targetDistance / (Mathf.Sin(2 * FiringAngle * Mathf.Deg2Rad) / gravity);

            float Vx = Mathf.Sqrt(projectileVelocity) * Mathf.Cos(FiringAngle * Mathf.Deg2Rad);
            float Vy = Mathf.Sqrt(projectileVelocity) * Mathf.Sin(FiringAngle * Mathf.Deg2Rad);

            initialVelocity = new Vector3(Vx, Vy, 0);
        }
    
    }
}
