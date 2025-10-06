using System;
using _Game.Core._Logger;
using _Game.Gameplay._BattleField.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class TargetDetection : MonoBehaviour
    {
        private const float TARGET_DISTANCE_EPSILON = 0.01f;
        
        [SerializeField] private TriggerObserver _attackZoneObserver;
        [SerializeField] private TriggerObserver _aggroZoneObserver;

        private ITargetRegistry _targetRegistry;
        private IMyLogger _logger;

        private Transform _self;
        private Faction _selfFaction;


        [ShowInInspector, ReadOnly]
        private float _aggroRadius;

        [ShowInInspector, ReadOnly]
        private float _attackRadius;

        [ShowInInspector, ReadOnly]
        private ITarget _currentTarget;

        [ShowInInspector, ReadOnly]
        private bool _isAttackable;

        [ShowInInspector, ReadOnly]
        private float _bodyRadius;

        private float _timeSinceLastScan = float.MaxValue;
        private const float SCAN_INTERVAL = 2f;

        public event Action<ITarget> AttackTargetChanged;
        public event Action<ITarget> OnPersecutionTargetChanged;

        public void Construct(
            ITargetRegistry targetRegistry,
            IMyLogger logger)
        {
            _logger = logger;
            _targetRegistry = targetRegistry;
        }
        
        public void SetTargetRegistry(ITargetRegistry targetRegistry) => 
            _targetRegistry = targetRegistry;

        public void Initialize(
            Transform self, 
            Faction selfFaction, 
            float aggroRadius, 
            float attackRadius,
            float bodyRadius,
            int attackLayer,
            int aggroLayer)
        {
            _bodyRadius = bodyRadius;
            _self = self;
            _selfFaction = selfFaction;
            _aggroRadius = aggroRadius;
            _attackRadius = attackRadius;
            
            _aggroZoneObserver.Initialize(aggroLayer);
            _attackZoneObserver.Initialize(attackLayer);
            
            _aggroZoneObserver.SetSize(aggroRadius);
            _attackZoneObserver.SetSize(attackRadius);
            
            _aggroZoneObserver.TriggerEnter += OnZoneEnter;
            _aggroZoneObserver.TriggerExit += OnZoneExit;
            
            _attackZoneObserver.TriggerExit  += OnZoneEnter;
            _attackZoneObserver.TriggerEnter += OnZoneExit;
        }

        public void Reset()
        {
            _timeSinceLastScan = float.MaxValue;
            _isAttackable = false;
            _currentTarget = null;
        }

        private void OnZoneExit(Collider2D obj)
        {
            _logger.Log("OnZoneExit", DebugStatus.Info);
            
            if(obj != null)
                Scan();
        }

        private void OnZoneEnter(Collider2D obj)
        {
            _logger.Log("OnZoneEnter", DebugStatus.Info);
            
            if(obj != null)
                Scan();
        }


        public void CleanUp()
        {
            _aggroZoneObserver.TriggerEnter -= OnZoneEnter;
            _aggroZoneObserver.TriggerExit -= OnZoneExit;
            
            _attackZoneObserver.TriggerEnter += OnZoneEnter;
            _attackZoneObserver.TriggerExit  += OnZoneExit;
        }

        public void Enable()
        {
            _aggroZoneObserver.SetActive(true);
            _attackZoneObserver.SetActive(true);
        }

        public void GameUpdate(float deltaTime)
        {
            _timeSinceLastScan += deltaTime;
            if (_timeSinceLastScan < SCAN_INTERVAL)
                return;

            Scan();
        }

        private void Scan()
        {
            _timeSinceLastScan = 0f;

            Vector3 origin = _self.position;

            float minDistance = float.MaxValue;
            ITarget closest = null;
            bool attackable = false;

            foreach (var target in _targetRegistry.GetAlive())
            {
                if (target.Faction == _selfFaction)
                    continue;

                float distance = Vector3.Distance(origin, target.Transform.position);
                float scanRadius = Mathf.Max(_aggroRadius, _attackRadius) + target.BodySize + TARGET_DISTANCE_EPSILON;

                if (distance <= scanRadius && distance < minDistance)
                {
                    minDistance = distance;
                    closest = target;

                    float attackThreshold = _attackRadius + target.BodySize + TARGET_DISTANCE_EPSILON;
                    attackable = distance <= attackThreshold;
                }
            }
            
            bool targetChanged = _currentTarget != closest;
            bool attackStateChanged = _isAttackable != attackable;

            if (targetChanged)
            {
                _currentTarget = closest;

                OnPersecutionTargetChanged?.Invoke(_currentTarget);
                AttackTargetChanged?.Invoke(attackable ? _currentTarget : null);
            }
            else if (attackStateChanged)
            {
                AttackTargetChanged?.Invoke(attackable ? _currentTarget : null);
            }

            _isAttackable = attackable;
        }

        public bool TryGetTargetForAttack(out ITarget target)
        {
            if (_isAttackable && IsTargetWithin(_attackRadius, _currentTarget))
            {
                target = _currentTarget;
                return true;
            }

            target = null;
            return false;
        }

        public bool TryGetTargetForPersecution(out ITarget target)
        {
            if (_currentTarget != null && IsTargetWithin(_aggroRadius, _currentTarget))
            {
                target = _currentTarget;
                return true;
            }

            target = null;
            return false;
        }

        private bool IsTargetWithin(float threshold, ITarget target)
        {
            if (target is not { IsActive: true })
                return false;

            float distance = Vector3.Distance(_self.position, target.Transform.position);
            float limit = threshold + target.BodySize + TARGET_DISTANCE_EPSILON;
            return distance <= limit;
        }
        
        public void Disable()
        {
            _aggroZoneObserver.SetActive(false);
            _attackZoneObserver.SetActive(false);
        }
    }
}