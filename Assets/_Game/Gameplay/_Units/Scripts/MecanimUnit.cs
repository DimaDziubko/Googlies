using _Game.Core._Logger;
using _Game.Core.Services.Random;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts.Attack;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class MecanimUnit : UnitBase
    {
        [SerializeField, Required] private DamageFlashEffectWithSpriteRenderer _damageFlash;
        [SerializeField, Required] private SortingOrderWithSortingGroup _dynamicSortingOrder;
        [SerializeField, Required] private MecanimUnitAnimator _animator;

        public override BaseUnitAnimator Animator => _animator;
        public override DamageFlashEffect DamageFlash => _damageFlash;
        public override DynamicSortingOrder DynamicSortingOrder => _dynamicSortingOrder;

        public override void Construct(
            IMyLogger logger,
            ITargetRegistry targetRegistry,
            IUnitData unitData, 
            Faction faction,
            IRandomService random)
        {
            base.Construct(logger, targetRegistry, unitData, faction, random);
            
            _animator.Initialize(logger);
            _animator.SetAttackSpeed(unitData.AttackPerSecond);
        }

        public override void Initialize(IShootProxy shootProxy, IVFXProxy vFXProxy, float speedFactor,
            IUnitEventsObserver eventsObserver)
        {
            base.Initialize(shootProxy, vFXProxy, speedFactor, eventsObserver);
            _damageFlash.Initialize();
        }

        public override void Reset()
        {
            base.Reset();
            _damageFlash.Reset();
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            _damageFlash.Cleanup();
            _animator.CleanUp();
        }

        public override void SetPaused(in bool isPaused)
        {
            base.SetPaused(in isPaused);
            _animator.SetPaused(isPaused);
        }

        public override void SetSpeedFactor(float speedFactor)
        {
            base.SetSpeedFactor(speedFactor);
            _animator.SetSpeedFactor(speedFactor);
        }

#if UNITY_EDITOR
        [Button]
        protected override void ManualInit()
        {
            base.ManualInit(); 
            
            _animator = GetComponentInChildren<MecanimUnitAnimator>();
            _dynamicSortingOrder = GetComponent<SortingOrderWithSortingGroup>();
            _damageFlash = GetComponent<DamageFlashEffectWithSpriteRenderer>();
        }
#endif
    }
}