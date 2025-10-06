using _Game.Gameplay._Weapon.Scripts;
using Sirenix.OdinInspector;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class SpineUnitEventListener : MonoBehaviour
    {
        [SerializeField, Required] private SkeletonAnimation _skeletonAnimation;
        
        [SerializeField, Required] private Weapon _weapon;

        [SerializeField] private bool _hasAggro = false;
        
        [ShowIf(nameof(_hasAggro))]
        [SerializeField] private UnitAggro _aggro;

        public void Initialize()
        {
            if (_skeletonAnimation != null)
                _skeletonAnimation.AnimationState.Event += HandleAnimationEvent;
        }

        public void Dispose()
        {
            if (_skeletonAnimation != null)
                _skeletonAnimation.AnimationState.Event -= HandleAnimationEvent;
        }

        private void HandleAnimationEvent(TrackEntry trackEntry, Spine.Event e)
        {
            if (e.Data.Name == "Attack") PerformAttack();
            else if (e.Data.Name == "Aggro") PerformAggro();
            else if (e.Data.Name == "SpecialAttack") PerformSpecialAttack();
            else if (e.Data.Name == "WeaponEnabled") PerformWeaponEnabled();
        }

        private void PerformWeaponEnabled() => _weapon?.EnableWeapon();
        
        private void PerformSpecialAttack() => _weapon?.SpecialAttack();

        private void PerformAggro() => _aggro?.OnAggro();

        private void PerformAttack() => _weapon?.Attack();
        
#if UNITY_EDITOR
        [Button]
        private void ManualInit()
        {
            _weapon = GetComponentInChildren<Weapon>();
            _skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
        }
#endif
        
    }
}