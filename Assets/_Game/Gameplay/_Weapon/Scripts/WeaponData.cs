using _Game.Core.Services.Audio;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Units.Scripts._Animation;
using _Game.Gameplay._Weapon.Factory;
using UnityEngine;

namespace _Game.Gameplay._Weapon.Scripts
{
    public class WeaponData : IWeaponData
    {
        public WeaponType WeaponType { get; private set; }
        public string ProjectileKey { get; private set; }
        public int WeaponId { get; private set; }
        public float ProjectileSpeed { get; private set; }
        public float TrajectoryWarpFactor { get; private set; }
        public float SplashRadius { get; private set; }
        public string ProjectileExplosionKey { get; private set; }
        public string MuzzleKey { get; private set; }
        public float Damage { get; private set; }
        public float DamageBoost { get; private set; }
        public int Layer { get; private set; }
        public int CollisionMask { get; private set; }
        public float SplashDamageRatio { get; private set; }
        public TrajectoryType TrajectoryType { get; private set; }
        public ProjectileImpactType ProjectileImpactType { get; private set; }
        public SoundData AttackSfx { get; private set; }
        public SoundData SpecialAttackSfx { get; private set; }

        public AimingSettings AimingSettings { get; private set; }

        public bool IsAiming { get; private set; }

        public float ImpulseStrength { get; private set; }

        public SoundData ProjectileSfx { get; private set; }
        public SoundData WeaponEnableSfx { get; private set; }
        public Color Color { get; private set; }

        public class WeaponDataBuilder
        {
            WeaponType _weaponType;
            string _projectileKey;
            int _weaponId;
            float _projectileSpeed;
            float _trajectoryWarpFactor;
            float _splashRadius;
            string _projectileExplosionKey;
            string _muzzleKey;
            float _damage;
            float _impulseStrength;
            float _damageBoost = 1;
            int _layer;
            int _collisionMask;
            float _splashDamageRatio;
            TrajectoryType _trajectoryType;
            ProjectileImpactType _projectileImpactType;

            SoundData _attackSfx;
            SoundData _projectileSfx;
            SoundData _weaponEnableSfx;
            SoundData _specialAttackSfx;

            AimingSettings _aimingSettings;

            Color _color;
            bool _isAiming;


            public WeaponDataBuilder WithWeaponType(WeaponType weaponType)
            {
                _weaponType = weaponType;
                return this;
            }

            public WeaponDataBuilder WithProjectileKey(string projectileKey)
            {
                _projectileKey = projectileKey;
                return this;
            }

            public WeaponDataBuilder WithWeaponId(int weaponId)
            {
                _weaponId = weaponId;
                return this;
            }

            public WeaponDataBuilder WithProjectileSpeed(float projectileSpeed)
            {
                _projectileSpeed = projectileSpeed;
                return this;
            }

            public WeaponDataBuilder WithTrajectoryWarpFactor(float trajectoryWarpFactor)
            {
                _trajectoryWarpFactor = trajectoryWarpFactor;
                return this;
            }

            public WeaponDataBuilder WithSplashRadius(float splashRadius)
            {
                _splashRadius = splashRadius;
                return this;
            }

            public WeaponDataBuilder WithProjectileExplosionKey(string projectileExplosionKey)
            {
                _projectileExplosionKey = projectileExplosionKey;
                return this;
            }

            public WeaponDataBuilder WithMuzzleKey(string muzzleKey)
            {
                _muzzleKey = muzzleKey;
                return this;
            }

            public WeaponDataBuilder WithDamage(float damage)
            {
                _damage = damage;
                return this;
            }
            
            public WeaponDataBuilder WithDamageBoost(float damageBoost)
            {
                _damageBoost = damageBoost;
                return this;
            }

            public WeaponDataBuilder WithLayer(int layer)
            {
                _layer = layer;
                return this;
            }

            public WeaponDataBuilder WithCollisionMask(int collisionMask)
            {
                _collisionMask = collisionMask;
                return this;
            }
            
            public WeaponDataBuilder WithSplashDamageRatio(float splashDamageRatio)
            {
                _splashDamageRatio = splashDamageRatio;
                return this;
            }
            
            public WeaponDataBuilder WithProjectileMoveType(TrajectoryType trajectoryType)
            {
                _trajectoryType = trajectoryType;
                return this;
            }
            
            public WeaponDataBuilder WithProjectileImpactType(ProjectileImpactType projectileImpactType)
            {
                _projectileImpactType = projectileImpactType;
                return this;
            }

            public WeaponDataBuilder WithAttackSfx(SoundData soundData)
            {
                _attackSfx = soundData;
                return this;
            }

            public WeaponDataBuilder WithSpecialAttackSfx(SoundData soundData)
            {
                _specialAttackSfx = soundData;
                return this;
            }

            public WeaponDataBuilder WithProjectileSfx(SoundData soundData)
            {
                _projectileSfx = soundData;
                return this;
            }

            public WeaponDataBuilder WithWeaponEnableSfx(SoundData soundData)
            {
                _weaponEnableSfx = soundData;
                return this;
            }

            public WeaponDataBuilder WithColor(Color color)
            {
                _color = color;
                return this;
            }
            
            public WeaponDataBuilder WithAiming(bool isAiming)
            {
                _isAiming = isAiming;
                return this;
            }
            
            public WeaponDataBuilder WithAimingSettings(AimingSettings settings)
            {
                _aimingSettings = settings;
                return this;
            }
            
            public WeaponDataBuilder WithImpulseStrength(float impulseStrength)
            {
                _impulseStrength = impulseStrength;
                return this;
            }

            public WeaponData Build()
            {
                var unitData = new WeaponData()
                {
                    WeaponType = _weaponType,
                    ProjectileKey = _projectileKey,
                    WeaponId = _weaponId,
                    ProjectileSpeed = _projectileSpeed,
                    TrajectoryWarpFactor = _trajectoryWarpFactor,
                    SplashRadius = _splashRadius,
                    ProjectileExplosionKey = _projectileExplosionKey,
                    MuzzleKey = _muzzleKey,
                    Damage = _damage,
                    Layer = _layer,
                    CollisionMask = _collisionMask,
                    DamageBoost = _damageBoost,
                    SplashDamageRatio = _splashDamageRatio,
                    TrajectoryType = _trajectoryType,
                    ProjectileImpactType = _projectileImpactType,
                    AttackSfx = _attackSfx,
                    SpecialAttackSfx = _specialAttackSfx,
                    ProjectileSfx = _projectileSfx,
                    WeaponEnableSfx = _weaponEnableSfx,
                    Color = _color,
                    AimingSettings = _aimingSettings,
                    IsAiming = _isAiming,
                    ImpulseStrength = _impulseStrength,
                };
                return unitData;
            }
        }
    }
}