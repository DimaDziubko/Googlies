using UnityEngine;

namespace _Game.Gameplay.Vfx.Scripts
{
    public class Shadow : VfxEntity
    {
        [SerializeField] private Transform _transform;
        
        private Vector3 _targetScale;
        private float _scalingSpeed;

        public Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }

        public Vector3 Scale
        {
            get => _transform.localScale;
            set => _transform.localScale = value;
        }
    }
}