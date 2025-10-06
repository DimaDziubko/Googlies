using _Game.Core._Logger;
using _Game.Core.Services.Random;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts._Animation;
using _Game.Gameplay._Units.Scripts.Attack;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class SpineUnit : UnitBase
    {
        [SerializeField, Required] private DamageFlashEffectWithMeshRenderer _damageFlash;
        [SerializeField, Required] private SortingOrderWithMeshRenderer _dynamicSortingOrder;
        [SerializeField, Required] private SpineUnitAnimator _animator;
        
        [SerializeField, Required] private SkinInitializer _skinInitializer;
        [SerializeField, Required] private SpineUnitEventListener _eventListener;

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
            _animator.Initialize(logger);
            _animator.SetAttackSpeed(unitData.AttackPerSecond);
            
            base.Construct(logger, targetRegistry, unitData, faction, random);
        }
        
        public override void Initialize(
            IShootProxy shootProxy, 
            IVFXProxy vFXProxy, 
            float speedFactor,
            IUnitEventsObserver eventsObserver)
        {
            base.Initialize(shootProxy, vFXProxy, speedFactor, eventsObserver);
            _skinInitializer.Initialize(CurrentSkin);
            _eventListener.Initialize();
            _damageFlash.Initialize();
        }

        public override void Recycle()
        {
            base.Recycle();
            _eventListener.Dispose();
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

        public override void CalmDown()
        {
            base.CalmDown();
            _animator.TryCalmDown();
        }

#if UNITY_EDITOR
        [Button]
        protected override void ManualInit()
        {
            base.ManualInit();
            _animator = GetComponentInChildren<SpineUnitAnimator>();
            _dynamicSortingOrder = GetComponent<SortingOrderWithMeshRenderer>();
            _damageFlash = GetComponent<DamageFlashEffectWithMeshRenderer>();
        }
#endif
    }
}