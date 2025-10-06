using System;
using _Game.Gameplay._Weapon.Factory;
using UnityEngine.Serialization;

namespace _Game.Core.Configs.Models._WeaponConfig
{
    [Serializable]
    public class RemoteWeaponConfig
    {
        public int Id;
        public float Damage;
        public float ProjectileSpeed;
        public float TrajectoryWarpFactor;
        public float SplashRadius;
        public float PlayerDamageMultiplier;
        public float EnemyDamageMultiplier;
        public float SplashDamageRatio;
        public string FBConfigId;
        
        public ProjectileImpactType ProjectileImpactType;
        [FormerlySerializedAs("ProjectileMoveType")] public TrajectoryType trajectoryType;
    }
}