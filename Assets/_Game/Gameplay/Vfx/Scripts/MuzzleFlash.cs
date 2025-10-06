using UnityEngine;

namespace _Game.Gameplay.Vfx.Scripts
{
    public class MuzzleFlash : KeyedVfxEntity
    {
        [SerializeField] private Transform _transform;
        [SerializeField] private float _duration = 0.2f;

        private Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }
        
        private Quaternion Rotation
        {
            get => _transform.rotation;
            set => _transform.rotation = value;
        }
        
        private float _age;
        
        public void Initialize(Vector3 position, Vector3 direction)
        {
            Position = position;
            Rotation = Quaternion.LookRotation(direction);
            _age = 0;
        }

        public override bool GameUpdate(float deltaTime)
        {
            _age += deltaTime;
            if (_age >= _duration)
            {
                Recycle();
                return false;
            }
            
            return true;
        }
    }
}