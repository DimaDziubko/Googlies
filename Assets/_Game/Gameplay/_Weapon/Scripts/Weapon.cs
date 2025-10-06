using _Game.Core._Logger;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Scripts
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField, Required] private Transform _projectileGenerator;
        [SerializeField, Required] private WeaponColorizer _colorizer;
        
        public bool IsAiming;
        [ShowIf( "IsAiming")]
        [SerializeField] private Transform _aimBoneTransform;
        [ShowIf( "IsAiming")]
        [SerializeField] private Transform _weaponBoneTransform;

        private WeaponStrategy _weaponStrategy;
        private IMyLogger _logger;

        public void Construct(WeaponStrategy weaponStrategy, IMyLogger logger)
        {
            _weaponStrategy = weaponStrategy;
            _logger = logger;
        }

        public void SetStrategy(WeaponStrategy weaponStrategy) => 
            _weaponStrategy = weaponStrategy;

        public void Initialize(
            IWeaponData weaponData,
            IVFXProxy vfxProxy,
            IShootProxy shootProxy,
            Faction faction)
        {
            _weaponStrategy.Initialize(
                weaponData,
                vfxProxy,
                shootProxy,
                _projectileGenerator,
                _aimBoneTransform,
                _weaponBoneTransform,
                _colorizer, 
                faction,
                _logger);
        }

        public Vector3 GetWeaponScale() => 
            _weaponStrategy.GetWeaponScale();
        
        public Vector3 SetWeaponScale(Vector3 scale) => 
            _weaponStrategy.SetWeaponScale(scale);
        
        public IWeaponData GetWeaponData() => 
            _weaponStrategy.GetWeaponData();

        public void SetData(IWeaponData weaponData) => 
            _weaponStrategy.SetWeaponData(weaponData);

        public void SetTarget(ITarget target) => _weaponStrategy.SetTarget(target);

        public void EnableWeapon() => _weaponStrategy.EnableWeapon();

        public void Attack() => _weaponStrategy.Attack();

        public void SpecialAttack() => _weaponStrategy.SpecialAttack();

        public void DisableWeapon() => _weaponStrategy.DisableWeapon();

        public void GameUpdate(float deltaTime) => _weaponStrategy.GameUpdate(deltaTime);
        public void LateGameUpdate(float deltaTime) => _weaponStrategy.LateGameUpdate(deltaTime);
        public void SetPaused(in bool isPaused) => _weaponStrategy.SetPaused(isPaused);
        public void Enable() => _weaponStrategy.Enable();
        public void Disable() => _weaponStrategy.Disable();
    }
}