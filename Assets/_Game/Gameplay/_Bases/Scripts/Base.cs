using System;
using _Game.Core.Services._Camera;
using _Game.Gameplay._Bases.Factory;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Units.Scripts.Attack;
using _Game.Gameplay.Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Bases.Scripts
{
    [RequireComponent(typeof(TargetPoint))]
    public class Base : GameBehaviour
    {
        [SerializeField, Required] private Transform _transform;
        [SerializeField, Required] private Health _health;
        [SerializeField, Required] private BoxCollider2D _bodyCollider;
        [SerializeField, Required] private TargetPoint _targetPoint;
        [SerializeField, Required] private BaseDestructionAnimator _animator;
        [SerializeField, Required] private DamageFlashEffect _damageFlash;
        
        [SerializeField, Required] private Transform _damageTextSpawnPoint;
        [SerializeField, Required] private Transform _targetPointTransform;
        
        private IBaseEventHandler _baseEventHandler;
        private ITargetRegistry _targetRegistry;

        public Faction Faction { get; private set; }
        public string Key { get; set; }

        public float CoinsPerBase => _coinsPerBase;
        public float CoinsState => _coinsState;


        [ShowInInspector] private float _coinsPerBase;

        [ShowInInspector] private float _coinsState;

        [ShowInInspector] private float _coinsPerHp;
        
        public Vector3 DamageTextPosition => _damageTextSpawnPoint.position;

        public Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }

        private Quaternion Rotation
        {
            get => _transform.rotation;
            set
            {
                _transform.rotation = value;
                _health.RotateToCamera();
            }
        }

        public IBaseFactory OriginFactory { get; set; }

        public void Construct(
            ITargetRegistry targetRegistry,
            float health,
            float coinsPerBase,
            IWorldCameraService cameraService,
            Faction faction,
            int baseLayer)
        {
            Faction = faction;
            
            _targetRegistry = targetRegistry;

            _bodyCollider.gameObject.layer = baseLayer;

            _coinsState = _coinsPerBase = coinsPerBase;

            _health.Construct(
                health,
                cameraService);

            _targetPoint.Transform = _transform;
            _targetPoint.Damageable = _health;
            _targetPoint.Faction = faction;
            _targetPoint.BodySize = _bodyCollider.size.x + _bodyCollider.offset.x;
            _targetPoint.Collider = _bodyCollider;

            _targetRegistry.Register(_targetPoint);
            
            _animator.Construct();

            _damageFlash.Initialize();

            HideHealth();
        }

        public void Init()
        {
            _health.Death += OnBaseDeath;
            _health.Hit += OnBaseHit;
        }

        public void PrepareIntro(
            IBaseEventHandler baseEventHandler,
            Vector3 position,
            Quaternion rotation)
        {
            _baseEventHandler = baseEventHandler;
            Position = position;
            Rotation = rotation;
        }

        public override void SetPaused(in bool isPaused) { }
        public void UpdateHealth(float health) => _health.UpdateData(health);

        public void ShowHealth() => _health.ShowHealth();

        private void HideHealth() => _health.HideHealth();

        public override void Recycle()
        {
            Unsubscribe();
            OriginFactory.Reclaim(this);
        }

        private void OnDestroy() => 
            _targetRegistry.Unregister(_targetPoint);
        
        private void OnBaseDeath()
        {
            _baseEventHandler.OnBaseDestructionStarted(this);

            _animator.AnimationCompleted += HandleAnimationCompleted;
            _animator.StartDestructionAnimation();

            Unsubscribe();

            _damageFlash.Cleanup();

            HideHealth();
        }

        private void Unsubscribe()
        {
            _health.Death -= OnBaseDeath;
            _health.Hit -= OnBaseHit;
        }

        private void HandleAnimationCompleted()
        {
            _baseEventHandler.OnBaseDestructionCompleted(this);
            _animator.AnimationCompleted -= HandleAnimationCompleted;
        }

        public void SpendCoins(float amount)
        {
            _coinsState -= amount;
            if (_coinsState <= 0) _coinsState = 0;
        }

        public override void Reset()
        {
            _health.ResetHealth();
            _animator.ResetSelf();
            HideHealth();
        }

        public void UpdateCoins(float coins) => _coinsPerBase = _coinsState = coins;

        private void OnBaseHit(float damage, float maxHealth)
        {
            _baseEventHandler.OnBaseHit(this, damage, maxHealth);
        }

#if UNITY_EDITOR

        //Helper


        [Button]
        private void ManualInit()
        {
            _transform = GetComponent<Transform>();
            _damageFlash = GetComponent<DamageFlashEffect>();
            _health = GetComponentInChildren<Health>();
            _bodyCollider = GetComponentInChildren<BoxCollider2D>();
            _targetPoint = GetComponent<TargetPoint>();
        }
#endif
    }
}