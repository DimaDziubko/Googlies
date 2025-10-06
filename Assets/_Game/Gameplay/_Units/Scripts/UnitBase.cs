using _Game.Core._Logger;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Services.Random;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Units.FSM;
using _Game.Gameplay._Units.FSM.States;
using _Game.Gameplay._Units.Scripts.Attack;
using _Game.Gameplay._Units.Scripts.Movement;
using _Game.Gameplay._Weapon.Scripts;
using _Game.Gameplay.Common;
using Pathfinding.RVO;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace _Game.Gameplay._Units.Scripts
{
    public abstract class UnitBase : GameBehaviour
    {
        [SerializeField, Required] private Transform _transform;
        [SerializeField, Required] private Health _health;
        [SerializeField, Required] private AUnitMove _aMove;
        [SerializeField, Required] private CircleCollider2D _bodyCollider;
        [SerializeField, Required] private TargetPoint _targetPoint;
        [SerializeField, Required] private RVOController _rVOController;
        [SerializeField, Required] private Transform _damageTextSpawnPoint;
        [SerializeField, Required] private Pivot _pivot;
        [SerializeField, Required] private UnitBasePusher _pusher;
        [SerializeField, Required] private UnitEventDispatcher _dispatcher;
        [SerializeField, Required] private Transform _skillEffectParent;
        [SerializeField, Required] private WarriorObjectPositioner _positioner;
        [SerializeField, Required] private TargetDetection _targetDetection;
        [SerializeField, Required] private Weapon _weapon;        
        [SerializeField, Required] private DissolveEffect _dissolveEffect;
        [SerializeField, Required] private PushComponent _pushComponent;

        public WarriorObjectPositioner Positioner => _positioner;
        public Health Health => _health;
        public PushComponent PushComponent => _pushComponent;
        public UnitEventDispatcher Dispatcher => _dispatcher;
        public AUnitMove AMove => _aMove;
        public abstract BaseUnitAnimator Animator { get; }
        public abstract DamageFlashEffect DamageFlash { get; }
        public abstract DynamicSortingOrder DynamicSortingOrder { get; }
        public Transform SkillEffectParent => _skillEffectParent;
        public Transform Transform => _transform;
        public Pivot Pivot => _pivot;
        public UnitBasePusher Pusher => _pusher;
        public DebuffMediator DebuffMediator { get; private set; }
        public Weapon Weapon => _weapon;
        public Faction Faction { get; private set; }
        public UnitType Type => UnitData.Type;
        public float CoinsPerKill => UnitData.CoinsPerKill;
        
        private IRandomService _random;
        public Vector3 DamageTextPosition => _damageTextSpawnPoint.position;
        public Vector3 DeathPosition { get; private set; }
        protected Skin CurrentSkin => UnitData.Skin;

        public Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }

        public Quaternion Rotation
        {
            get => _transform.rotation;
            set => _transform.rotation = value;
        }

        private Vector3 _destination;

        public Vector3 Destination
        {
            get => _destination;
            set => _destination = value;
        }

        private float _tempAnimatorSpeedFactor;

        private float _tempMoveSpeedFactor;

        [ShowInInspector, ReadOnly]
        private WarriorFsm _warriorFsm;

        public WarriorFsm WarriorFsm => _warriorFsm;

        private bool _isDead;
        protected bool _isReadyToRecycle;

        public IUnitFactory OriginFactory { get; set; }
        
        protected bool _isFrozen;

        private IVFXProxy _vfxProxy;
        private IMyLogger _logger;

        public TargetDetection TargetDetection => _targetDetection;
        
        public IUnitData UnitData { get; private set; }


        public virtual void Construct(
            IMyLogger logger,
            ITargetRegistry targetRegistry,
            IUnitData unitData,
            Faction faction,
            IRandomService random)
        {
            _logger = logger;
            _targetRegistry = targetRegistry;

            _targetDetection.Construct(targetRegistry, _logger);
            
            _targetDetection.Initialize(
                _transform, 
                faction, 
                1, 
                unitData.AttackDistance,
                _bodyCollider.radius,
                unitData.AttackLayer,
                unitData.AggroLayer);
            
            UnitData = unitData;
            
            _rVOController.collidesWith = unitData.RVOLayer;
            _rVOController.layer = unitData.RVOLayer;
            gameObject.layer = unitData.Layer;

            Faction = faction;
            _random = random;
            
            _health.Death += OnDeath;
            _health.Hit += OnHit;
            _pusher.OnUnitPushedOut += OnPushOut;
            
            _health.HideHealth();

            _targetPoint.Damageable = _health;
            _targetPoint.Transform = _transform;
            _targetPoint.Faction = faction;
            _targetPoint.BodySize = _bodyCollider.radius;
            _targetPoint.Collider = _bodyCollider;
            _targetPoint.Pushable = _pushComponent;
            
            targetRegistry.Register(_targetPoint);

            DebuffMediator = new DebuffUnitMediator(this);

            _pusher.Construct(_aMove);
            
            InitializeFsm();
        }

        public void SetUnitData(IUnitData unitData)
        {
            UnitData = unitData;

            _rVOController.collidesWith = unitData.RVOLayer;
            _rVOController.layer = unitData.RVOLayer;
            gameObject.layer = unitData.Layer;

            _health.SetMaxHealth(unitData.Health);
        }

        private ITargetRegistry _targetRegistry;


        public virtual void Initialize(
            IShootProxy shootProxy,
            IVFXProxy vFXProxy,
            float speedFactor,
            IUnitEventsObserver eventsObserver)
        {
            _vfxProxy = vFXProxy;
            
            _weapon.Initialize(UnitData.WeaponData, vFXProxy, shootProxy, Faction);
            _dissolveEffect.Initialize();

            SetSpeedFactor(speedFactor);

            _dispatcher.RegisterEventObserver(eventsObserver);
        }

        public override void Reset()
        {
            _isDead = false;
            _isReadyToRecycle = false;
            _health.ResetHealth();
            _health.HideHealth();
            _targetDetection.Reset();
            _targetDetection.Enable();
            _aMove.Enable();
            _rVOController.enabled = true;
            
            _weapon.Enable();
            _warriorFsm.SetStateByType(typeof(IdleWarriorState));
            _isFrozen = false;
            _aMove.Cleanup();
        }
        
        public override bool GameUpdate(float deltaTime)
        {
            if (_isReadyToRecycle)
            {
                Recycle();
                return false;
            }
            
            _targetDetection.GameUpdate(deltaTime);
            
            _warriorFsm.GameUpdate(deltaTime);
            
            DynamicSortingOrder.GameUpdate();
            Animator.GameUpdate(deltaTime);
            _pusher.GameUpdate(deltaTime);
            _weapon.GameUpdate(deltaTime);
            
            return true;
        }

        public override bool LateGameUpdate(float deltaTime)
        {
            _weapon.LateGameUpdate(deltaTime);
            _health.RotateToCamera();
            Animator.LateGameUpdate(deltaTime);
            return true;
        }
        
        public virtual void CalmDown() {}

        public override void Recycle()
        {
            if (!_isDead)
            {
                OnDeath();
                return;
            }
            
            OriginFactory.Reclaim(this);
        }
        
        private void InitializeFsm()
        {
            _warriorFsm = new WarriorFsm();

            IdleWarriorState idleState = new IdleWarriorState(this, "Idle");
            MoveToTargetWarriorState moveToTargetState = new MoveToTargetWarriorState(this, "MoveToTarget");
            MoveToPointWarriorState moveToPointState = new MoveToPointWarriorState(this, _random, "MoveToPoint");
            AttackWarriorState attackState = new AttackWarriorState(this, "Attack");
            DeathWarriorState deathState = new DeathWarriorState(this, "Death");
            FrozenWarriorState frozenState = new FrozenWarriorState(this, "Frozen");
            
            At(idleState, attackState, new FuncPredicate(() => _targetDetection.TryGetTargetForAttack(out _)));
            At(moveToPointState, attackState, new FuncPredicate(() => _targetDetection.TryGetTargetForAttack(out _)));
            At(moveToTargetState, attackState, new FuncPredicate(() => _targetDetection.TryGetTargetForAttack(out _)));
            
            At(idleState, moveToTargetState, new FuncPredicate(() => !_targetDetection.TryGetTargetForAttack(out _) && _targetDetection.TryGetTargetForPersecution(out _)));
            At(moveToPointState, moveToTargetState, new FuncPredicate(() => !_targetDetection.TryGetTargetForAttack(out _) && _targetDetection.TryGetTargetForPersecution(out _)));
            At(attackState, moveToTargetState, new FuncPredicate(() => !_targetDetection.TryGetTargetForAttack(out _) && _targetDetection.TryGetTargetForPersecution(out _)));
            At(frozenState, idleState, new FuncPredicate(() => !_isFrozen ));

            Any(frozenState, new FuncPredicate(TransitionToFrozenState));
            Any(deathState, new FuncPredicate(() => _isDead));
            Any(idleState, new FuncPredicate(ReturnToIdleState));
            Any(moveToPointState, new FuncPredicate(ReturnToMoveToPointState));

            // Set initial state
            _warriorFsm.SetState(idleState);
        }
        
        private void At(IWarriorState from, IWarriorState to, IPredicate condition) => _warriorFsm.AddTransition(from, to, condition);
        private void Any(IWarriorState to, IPredicate condition) => _warriorFsm.AddAnyTransition(to, condition);
        
        private bool TransitionToFrozenState() => 
            _isFrozen && !_isDead;

        private bool ReturnToIdleState()
        {
            return !_isFrozen
                   && !_isDead
                   && !_targetDetection.TryGetTargetForPersecution(out _)
                   && !_targetDetection.TryGetTargetForAttack(out _)
                   && Vector3.Distance(Position, Destination) < float.Epsilon;
        }
        
        private bool ReturnToMoveToPointState()
        {
            return !_isFrozen
                   && !_isDead
                   && !_targetDetection.TryGetTargetForPersecution(out _)
                   && !_targetDetection.TryGetTargetForAttack(out _)
                   && Vector3.Distance(Position, Destination) > float.Epsilon;
        }

        private void OnDeath() => _isDead = true;

        public void HandleDeath()
        {
            _health.HideHealth();
            _vfxProxy.SpawnUnitVfx(Position);
            DeathPosition = Position;
            
            _targetDetection.Disable();
            _weapon.Disable();
            _rVOController.enabled = false;
            _aMove.Disable();
            _aMove.Cleanup();

            _dispatcher.NotifyDeath(this);
            _dispatcher.Cleanup();

            DebuffMediator?.Dispose();

            Animator.PlayDeath(() =>
            {
                _dissolveEffect.CallDissolve(() =>
                {
                    Animator.ResetPose();
                    _isReadyToRecycle = true;
                });
            });
        }
        
        private void OnDestroy()
        {
            Cleanup();
        }

        protected virtual void Cleanup()
        {
            _dissolveEffect.Cleanup();
            _dispatcher.Cleanup();
 
            _targetRegistry.Unregister(_targetPoint);
            
            _warriorFsm.Cleanup();
            
            _health.Death -= OnDeath;
            _health.Hit -= OnHit;
            _pusher.OnUnitPushedOut -= OnPushOut;
            _targetDetection.CleanUp();
            _weapon.SetTarget(null);
        }

        private void OnPushOut() => 
            _dispatcher.OnPushOut(this);

        private void OnHit(float damage, float maxHealth) =>
            _dispatcher.NotifyHit(this, damage);

        public override void SetPaused(in bool isPaused)
        {
            _aMove.SetPaused(isPaused);
            _weapon.SetPaused(isPaused);
        }
        
        public void SetFrozen(bool isFrozen)
        {
            if (isFrozen)
            {
                if (_isFrozen) return;
                _isFrozen = true;
                
                return;
            }

            if (!_isFrozen) return;
            _isFrozen = false;
        }


        public override void SetSpeedFactor(float speedFactor)
        {
            _aMove.SetSpeedFactor(speedFactor);
        }

        #region Helpers

#if UNITY_EDITOR

        [Button]
        protected virtual void ManualInit()
        {
            _transform = GetComponent<Transform>();
            _health = GetComponent<Health>();
            _rVOController = GetComponent<RVOController>();
            _weapon = GetComponentInChildren<Weapon>();
            _bodyCollider = GetComponent<CircleCollider2D>();
            _aMove = GetComponent<AUnitMove>();
            _targetPoint = GetComponent<TargetPoint>();
            _targetDetection = GetComponentInChildren<TargetDetection>();
            _pushComponent = GetComponent<PushComponent>();
            
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }

        [Button]
        private void ManualInitUnitSortingLayers()
        {
            SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var renderer in spriteRenderers)
            {
                renderer.sortingLayerID = SortingLayer.NameToID("Units");
            }

            SortingGroup[] sortingGroups = GetComponentsInChildren<SortingGroup>(true);
            foreach (var group in sortingGroups)
            {
                group.sortingLayerID = SortingLayer.NameToID("Units");
            }
        }
#endif

        #endregion

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}