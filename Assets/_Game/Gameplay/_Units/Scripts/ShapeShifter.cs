using System.Collections;
using _Game.Core._Logger;
using _Game.Core.Services.Random;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.FSM.States;
using _Game.Gameplay._Units.Scripts.Attack;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class ShapeShifter : UnitBase
    {
        [Header("Primary shape")]
        [SerializeField, Required] private SkinInitializer _skinInitializer;
        [SerializeField, Required] private SpineUnitEventListener _primaryShapeEventListener;
        [SerializeField, Required] private BaseUnitAnimator _primaryAnimator;
        [SerializeField, Required] private GameObject _primaryShape;
        [SerializeField, Required] private DamageFlashEffect _primaryDamageFlash;
        [SerializeField, Required] private DynamicSortingOrder _primaryDynamicSortingOrder;
        [SerializeField, Required] private SimpleVfxController _vfxController;
        
        [Header("Secondary shape")]
        [SerializeField, Required] private SpineUnitEventListener _secondaryShapeEventListener;
        [SerializeField, Required] private BaseUnitAnimator _secondaryAnimator;
        [SerializeField, Required] private GameObject _secondaryShape;
        [SerializeField, Required] private DamageFlashEffect _secondaryDamageFlash;
        [SerializeField, Required] private DynamicSortingOrder _secondaryDynamicSortingOrder;

        private bool IsPrimaryState {get; set; }

        private BaseUnitAnimator _activeAnimator;
        private DamageFlashEffect _activeDamageFlash;
        private DynamicSortingOrder _activeDynamicSortingOrder;
        
        public override BaseUnitAnimator Animator => _activeAnimator;
        public override DamageFlashEffect DamageFlash => _activeDamageFlash;
        public override DynamicSortingOrder DynamicSortingOrder => _activeDynamicSortingOrder;

        public override void Construct(
            IMyLogger logger,
            ITargetRegistry targetRegistry,
            IUnitData unitData, 
            Faction faction,
            IRandomService random)
        {
            base.Construct(logger, targetRegistry, unitData, faction, random);
            
            _primaryAnimator.Initialize(logger);
            _secondaryAnimator.Initialize(logger);
            
            _primaryAnimator.SetAttackSpeed(unitData.AttackPerSecond);
            _secondaryAnimator.SetAttackSpeed(unitData.AttackPerSecond);
        }
        
        public override void Initialize(
            IShootProxy shootProxy, 
            IVFXProxy vFXProxy, 
            float speedFactor,
            IUnitEventsObserver eventsObserver)
        {
            base.Initialize(shootProxy, vFXProxy, speedFactor, eventsObserver);

            _skinInitializer.Initialize(CurrentSkin);
            
            _primaryShapeEventListener.Initialize();
            _secondaryShapeEventListener.Initialize();
            
            _primaryDamageFlash.Initialize();
            _secondaryDamageFlash.Initialize();

            ShiftShape(true);
        }

        [Button]
        public void ShiftShape(bool isPrimary)
        {
            if(IsPrimaryState == isPrimary) return;
            
            IsPrimaryState = isPrimary;
            
            if (isPrimary)
            {
                _vfxController.PlayParticleEffect();
                _activeAnimator = _primaryAnimator;
                _primaryShape.SetActive(true);
                _secondaryShape.SetActive(false);
                _activeDamageFlash = _primaryDamageFlash;
                _activeDynamicSortingOrder = _primaryDynamicSortingOrder;
                return;
            }
            
            StartCoroutine(PlayVfxAndShiftShapeCoroutine(isPrimary));
        }
        
        private IEnumerator PlayVfxAndShiftShapeCoroutine(bool isPrimary)
        {
            _vfxController.PlayParticleEffect();
            
            yield return new WaitForSeconds(0.5f);
    
            _activeAnimator = _secondaryAnimator;
            _primaryShape.SetActive(false);
            _secondaryShape.SetActive(true);
            _activeDamageFlash = _secondaryDamageFlash;
            _activeDynamicSortingOrder = _secondaryDynamicSortingOrder;
            WarriorFsm.SetStateByType(typeof(IdleWarriorState));
        }
        
        public override void Recycle()
        {
            base.Recycle();
            _primaryShapeEventListener.Dispose();
            _secondaryShapeEventListener.Dispose();
        }

        public override void CalmDown()
        {
            if(IsPrimaryState) return;
            //if(AggroDetectionOld.HasTarget || AttackDetectionOld.HasTarget) return;
            if(TargetDetection.TryGetTargetForPersecution(out _) || TargetDetection.TryGetTargetForAttack(out _)) return;
            
            base.CalmDown();
            
            _primaryAnimator.TryCalmDown();
            _secondaryAnimator.TryCalmDown();
            
            ShiftShape(true);
        }

        public override void SetPaused(in bool isPaused)
        {
            base.SetPaused(in isPaused);
            
            _primaryAnimator.SetPaused(isPaused);
            _secondaryAnimator.SetPaused(isPaused);
        }
        
        public override void Reset()
        {
            base.Reset();
            _primaryDamageFlash.Reset();
            _secondaryDamageFlash.Reset();
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            
            _primaryDamageFlash.Cleanup();
            _secondaryDamageFlash.Cleanup();
            
            _primaryAnimator.CleanUp();
            _secondaryAnimator.CleanUp();
        }
        

        public override void SetSpeedFactor(float speedFactor)
        {
            base.SetSpeedFactor(speedFactor);
            
            _primaryAnimator.SetSpeedFactor(speedFactor);
            _secondaryAnimator.SetSpeedFactor(speedFactor);
        }

    }
}