using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay.Vfx.Scripts
{
    public class Wind : VfxEntity
    {
        [SerializeField, Required] private Transform _transform;
        [SerializeField, Required] ParticleSystem _windParticles;
        
        [SerializeField] private float _duration = 6f;

        private Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }

        private float _age;

        public void Initialize(Vector3 position)
        {
            Position = position;
            _age = 0;
        }

        // public void Play()
        // {
        //     _windParticles.Simulate(0, true, true);
        //     _windParticles.Play();
        // }

        public override bool GameUpdate(float deltaTime)
        {
            _age += deltaTime;
            if (_age >= _duration)
            {
                OriginFactory.Reclaim(this);
                return false;
            }

            return true;
        }
    }
}