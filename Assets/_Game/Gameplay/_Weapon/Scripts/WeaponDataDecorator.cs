using _Game.Core.Services.Audio;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Units.Scripts._Animation;
using _Game.Gameplay._Weapon.Factory;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Scripts
{
    public abstract class WeaponDataDecorator : IWeaponData
    {
        protected readonly IWeaponData _weaponData;

        protected WeaponDataDecorator(IWeaponData weaponData)
        {
            _weaponData = weaponData;
        }

        public WeaponType WeaponType => _weaponData.WeaponType;
        public string ProjectileKey => _weaponData.ProjectileKey;
        public int WeaponId => _weaponData.WeaponId;
        public float ProjectileSpeed => _weaponData.ProjectileSpeed;
        public float TrajectoryWarpFactor => _weaponData.TrajectoryWarpFactor;
        public float SplashRadius => _weaponData.SplashRadius;
        public string ProjectileExplosionKey => _weaponData.ProjectileExplosionKey;
        public string MuzzleKey => _weaponData.MuzzleKey;
        public virtual float Damage => _weaponData.Damage;
        public virtual float DamageBoost => _weaponData.DamageBoost;
        public int Layer => _weaponData.Layer;
        public int CollisionMask => _weaponData.CollisionMask;
        public float SplashDamageRatio  => _weaponData.SplashDamageRatio;
        public TrajectoryType TrajectoryType => _weaponData.TrajectoryType;
        public ProjectileImpactType ProjectileImpactType => _weaponData.ProjectileImpactType;
        public SoundData AttackSfx => _weaponData.AttackSfx;
        public SoundData WeaponEnableSfx => _weaponData.WeaponEnableSfx;
        public SoundData ProjectileSfx => _weaponData.ProjectileSfx;
        public SoundData SpecialAttackSfx => _weaponData.SpecialAttackSfx;
        public AimingSettings AimingSettings => _weaponData.AimingSettings;

        public bool IsAiming => _weaponData.IsAiming;

        public float ImpulseStrength => _weaponData.ImpulseStrength;

        public Color Color => _weaponData.Color;
    }
}