using System.Collections;
using _Game.Core.Configs.Models._Skills;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Utils;
using _Game.Utils.MoveToTrail;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay.Vfx.Scripts
{
    public class Meteor : KeyedVfxEntity
    {
        [SerializeField, Required] private Transform _transform;
        [SerializeField, Required] private Rotator _rotator;
        [SerializeField, Required] private MoveToTrailUV _trail;
        [SerializeField, Required] private SpriteRenderer _view;
        
        private bool _isGrounded;

        private readonly Collider2D[] _hitBuffer = new Collider2D[5];
        
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
        
        private IVFXProxy _vFXProxy;

        private MeteorConfig _config;
        private Shadow _shadow;

        private float _damageToDeal;
        private Vector3 _destination;
        private float _speed;
        private Vector3 _shadowMaxScale;
        private float _initialDistance;

        private Coroutine _destroyCoroutine;
        
        public void Initialize(
            Vector3 position, 
            Vector3 destination,
            MeteorConfig config, 
            IVFXProxy vfxProxy,
            float damageToApply)
        {
            Position = position;
            _destination = destination;
            _vFXProxy = vfxProxy;
            _config = config;
            Scale = config.GetRandomScale();
            _speed = config.GetRandomSpeed();
            _damageToDeal = damageToApply;
            
            _rotator.SetSpeed(config.GetRandomRotationSpeed());
            _rotator.SetClockwise(config.GetRotationDirection());
            
            _trail.Initialize();
            
            _isGrounded = false;
            _view.enabled = true;
            
            _shadow = _vFXProxy.VfxFactory.Get<Shadow>();
            _shadow.Position = _destination;
            _shadow.Scale = Vector3.zero;
            _shadowMaxScale = new Vector3(Scale.x * 0.7f, (Scale.x * 0.7f) / 3, 1);
            _initialDistance = Vector3.Distance(Position, destination);
        }
        
        public override bool GameUpdate(float deltaTime)
        {
            _rotator.Rotate(deltaTime);
 
            Vector3 direction = (_destination - Position).normalized;
            float distance = _speed * deltaTime;
            
            float remainingDistance = Vector3.Distance(Position, _destination);
            
            float journeyProgress = 1 - Mathf.Clamp01(remainingDistance / _initialDistance);
            
            if (_isGrounded)
            {
                _shadow.Scale = _shadowMaxScale;
            }
            else
            {
                _shadow.Scale = Vector3.Lerp(Vector3.zero, _shadowMaxScale, journeyProgress);
            }

            if (remainingDistance <= distance && !_isGrounded)
            {
                OnDestinationReached();
                return false;
            }
            
            Position += direction * distance;
            
            return true;
        }

        public override bool LateGameUpdate(float deltaTime)
        {
            _trail.LateGameUpdate(deltaTime);
            return true;
        }
        
        private void OnDestinationReached()
        {
            _isGrounded = true;
            _view.enabled = false;
            _vFXProxy.SpawnMeteorExplosion(_config.ExplosionKey, Position).Forget();
            PlaySfx();
            ApplyDamage();
            _shadow.Recycle();
            _cameraService.Shake(_config.ShakeDuration, _config.ShakeMagnitude);
            _destroyCoroutine = StartCoroutine(DestroyAfterDelay(2f));
        }

        private void ApplyDamage()
        {
            var collisionMask = (1 << Constants.Layer.MELEE_ENEMY) |
                                (1 << Constants.Layer.RANGE_ENEMY);

            int count = Physics2D.OverlapCircleNonAlloc(
                Position, 
                _config.DamageRadius, 
                _hitBuffer,
                collisionMask);

            for (int i = 0; i < count; i++)
            {
                _targetRegistry.TryGetTarget(_hitBuffer[i], out var target);
                
                if (target is {IsActive: true })
                {
                    target.TakeDamage(_damageToDeal);
                }
            }
        }

        private IEnumerator DestroyAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Recycle();
        }
        
        private void PlaySfx()
        {
            if (_audioService != null && _config.ExplodeSound != null)
            {
                _audioService.PlayOneShot(_config.ExplodeSound);
            }
        }
        
        public override void Recycle()
        {
            if (_destroyCoroutine != null)
            {
                StopCoroutine(_destroyCoroutine);
                _destroyCoroutine = null;
            }
            
            base.Recycle();
        }
    }
}