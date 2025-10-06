using UnityEngine;

namespace _Game.Scenes.Tests
{
    public class Proj_2 : MonoBehaviour
    {
        public Transform MyTransform;
    
        private bool _isInited = false;

        public float speed = 5f; 
        public float frequency = 20f; 
        public float magnitude = 0.5f;
    
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
                Initialize();
            }
        }

        private float _amplitude;

        private void Initialize()
        {
            _isInited = true;
            float distance = Vector3.Distance(Position, _target.position);
            _amplitude = distance / 10;
        }


        private void Update()
        {
            if(!_isInited) return;

            var targetPosition = Target.position; 
        
            Vector3 direction = (targetPosition - Position).normalized;
            Vector3 perpendicular = Quaternion.Euler(0, 0, 90) * direction;
        
        
        
        
            //Position += (direction * speed + wave) * Time.deltaTime;
            Rotation = Quaternion.LookRotation(Vector3.forward, direction);
        }
    }
}
