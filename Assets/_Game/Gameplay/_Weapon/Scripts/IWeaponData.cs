using _Game.Core.Services.Audio;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Units.Scripts._Animation;
using _Game.Gameplay._Weapon.Factory;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Scripts
{
    public interface IWeaponData
    {
        public WeaponType WeaponType { get;}
        public string ProjectileKey { get;}
        public int WeaponId { get;}
        public float ProjectileSpeed { get;}
        public float TrajectoryWarpFactor { get;}
        public float SplashRadius { get; }
        public string ProjectileExplosionKey { get; }
        public string MuzzleKey { get; }
        public float Damage { get; }
        public float DamageBoost { get; }
        public int Layer { get; }
        public int CollisionMask { get; }
        float SplashDamageRatio { get;}
        TrajectoryType TrajectoryType { get;}
        ProjectileImpactType ProjectileImpactType { get;}
        SoundData AttackSfx { get;}
        Color Color { get;}
        SoundData WeaponEnableSfx { get;}
        SoundData ProjectileSfx { get;}
        SoundData SpecialAttackSfx { get;}
        AimingSettings AimingSettings { get; }
        bool IsAiming { get;}
        float ImpulseStrength { get;}
    }
}