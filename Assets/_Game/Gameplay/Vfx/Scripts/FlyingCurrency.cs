using _Game.Core.Services.Audio;
using _Game.Gameplay._Coins.Scripts;
using UnityEngine;

namespace _Game.Gameplay.Vfx.Scripts
{
    [RequireComponent(typeof(ParticleSystem))]
    public class FlyingCurrency : FlyingReward
    {
        private const float  EPSILON = 0.3f;
        
        [SerializeField] private ParticleSystem _system;
        [SerializeField] private float _delayBeforeMoving = 1.5f;
        [SerializeField] private float _initialSpreadRadius = 1.5f;
        [SerializeField] private float _speed = 30.0f;
        [SerializeField] private float _totalLifetime = 5.0f;
        [SerializeField] private int _maxParticles = 100;

        [SerializeField] private AudioClip _collectSfx;
        
        private IAudioService _audioService;

        private Vector3 _target;

        private ParticleSystem.Particle[] _particles;
        private float _activeTimer;

        public void Construct(IAudioService audioService)
        {
            _audioService = audioService;
        }
        
        public void Init(
            Vector3 position, 
            Vector3 animationTargetPoint, 
            int coinsToSpawn)
        {
            _system.Stop();
            Position = position;
            _target = animationTargetPoint;
            _particles = new ParticleSystem.Particle[_maxParticles];
            _system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            InitializeParticles(coinsToSpawn);
            _activeTimer = _totalLifetime;
        }
        private void InitializeParticles(int count)
        {
            _system.Emit(count);
            int emittedCount = _system.GetParticles(_particles);
            for (int i = 0; i < emittedCount; i++)
            {
                Vector3 randomDirection = Random.insideUnitSphere * _initialSpreadRadius;
                _particles[i].position += randomDirection;
            }
            _system.SetParticles(_particles, emittedCount);
        }


        void Update()
        {
            if (_activeTimer <= 0)
            {
                if (OriginFactory != null)
                {
                    Recycle();
                    return;
                }
            }
            _activeTimer -= Time.deltaTime;

            int count = _system.GetParticles(_particles);
            for (int i = 0; i < count; i++)
            {
                Vector3 particleWorldPosition = _system.transform.TransformPoint(_particles[i].position);
                float distanceToTarget = Vector3.Distance(particleWorldPosition, _target);

                if (_particles[i].remainingLifetime < _system.main.startLifetime.constant - _delayBeforeMoving || distanceToTarget < EPSILON)
                {
                    if (distanceToTarget < EPSILON)
                    {
                        _particles[i].remainingLifetime = 0;
                        PlayCollectSfx();
                    }
                    else
                    {
                        Vector3 directionToTarget = (_target - particleWorldPosition).normalized;
                        _particles[i].position += _system.transform.InverseTransformDirection(directionToTarget * (_speed * Time.deltaTime));
                    }
                }
            }

            _system.SetParticles(_particles, count);
        }

        private void PlayCollectSfx()
        {
            if (_audioService != null && _collectSfx != null)
            {
                _audioService.PlayOneShot(_collectSfx);
            }
        }
    }
}