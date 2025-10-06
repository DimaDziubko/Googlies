using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Scripts
{
    public abstract class WeaponStrategy
    {
        protected IWeaponData _weaponData;
        protected ITarget _target;

        protected readonly ISoundService _soundService;
        protected IVFXProxy _vfxProxy;
        protected IShootProxy _shootProxy;
        protected Transform _generator;
        protected WeaponColorizer _colorizer;
        protected Faction _faction;
        protected IMyLogger _logger;
        protected Transform _aim;
        protected Transform _weaponBone;

        protected Vector3 _weaponScale = Vector3.one;

        protected WeaponStrategy(ISoundService soundService)
        {
            _soundService = soundService;
        }
        
        public virtual void Initialize(
            IWeaponData weaponData,
            IVFXProxy vfxProxy,
            IShootProxy shootProxy,
            Transform generator,
            Transform aim,
            Transform weaponBone,
            WeaponColorizer colorizer,
            Faction faction,
            IMyLogger logger)
        {
            _weaponBone = weaponBone;
            _aim = aim;
            _faction = faction;
            _weaponData = weaponData;
            _vfxProxy = vfxProxy;
            _shootProxy = shootProxy;
            _colorizer = colorizer;
            _generator = generator;
            _logger = logger;
        }

        protected Vector3 GetWeaponPosition()
        {
            if(_weaponBone != null)
                return _weaponBone.position;
            return Vector3.zero;
        }
        public IWeaponData GetWeaponData() => _weaponData;
        public void SetWeaponData(IWeaponData weaponData) => _weaponData = weaponData;
        public void SetTarget(ITarget target) => _target = target;
        public virtual void EnableWeapon() { }
        public abstract void Attack();
        public virtual void DisableWeapon() { }
        public virtual void GameUpdate(float deltaTime) { }
        public virtual void SpecialAttack() { }
        public Vector3 GetWeaponScale() => _weaponScale;
        public Vector3 SetWeaponScale(Vector3 scale) => _weaponScale = scale;
        public virtual void SetPaused(in bool isPaused) { }
        public virtual void Enable() { }
        public virtual void Disable() { }
        public virtual void LateGameUpdate(float deltaTime) { }
    }
}