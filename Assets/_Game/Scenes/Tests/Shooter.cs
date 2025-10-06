using UnityEngine;

namespace _Game.Scenes.Tests
{
    public class Shooter : MonoBehaviour
    {
        [SerializeField] private Transform _transform;

        public Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }
    
        public Proj_2 Bullet;
        public Transform target;
        public float FiringAngle = 45f;
    
        [SerializeField] private Transform _p1;
        [SerializeField] private Transform _p2;
    
    
        // Update is called once per frame
        void Update()
        {
            float distance = Vector3.Distance(Position, target.position);
            Vector3 direction = (target.position - Position).normalized;
            Vector3 perpendicular = Quaternion.Euler(0, 0, 90) * direction;

            _p1.transform.position = Position + direction * distance * 0.3f + perpendicular * distance / 20;
            _p2.transform.position = Position + direction * distance * 0.7f + perpendicular * distance / 20;
        
        
            if (Input.GetButtonDown("Jump"))
            {
                var bullet = Instantiate(Bullet, transform.position, Quaternion.identity);
                bullet.Target = target;
                //bullet.FiringAngle = FiringAngle;
            }
        }
    }
}
