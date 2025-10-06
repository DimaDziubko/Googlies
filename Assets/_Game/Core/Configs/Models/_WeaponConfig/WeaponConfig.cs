using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Services.Audio;
using _Game.Gameplay._Units.Factory;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Units.Scripts._Animation;
using _Game.Gameplay._Weapon.Factory;
using _Game.Utils;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace _Game.Core.Configs.Models._WeaponConfig
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Configs/Weapon")]
    [Serializable]
    public class WeaponConfig : ScriptableObject
    {
        public int Id;
        public WeaponType WeaponType;

        [ValueDropdown("GetProjectileKeys"), ShowIf( "IsProjectileType")]
        public string ProjectileKey;
        
        public bool IsMuzzle;
        [ValueDropdown("GetMuzzleKeys"), ShowIf( "IsMuzzle")]
        public string MuzzleKey;

        public bool IsExplosion;
        [ValueDropdown("GetExplosionKeys"), ShowIf( "IsExplosion")]
        public string ProjectileExplosionKey;

        public float Damage;
        
        [ShowIf( "IsProjectileType")]
        public float ProjectileSpeed;

        [ShowIf( "IsProjectileType")]
        public float TrajectoryWarpFactor;
        
        [ShowIf( "IsSplash")]
        public float SplashRadius;

        [ShowIf( "IsSplash")]
        public float SplashDamageRatio = 0;

        [FormerlySerializedAs("ProjectileMoveType")] [ShowIf( "IsProjectileType")]
        public TrajectoryType trajectoryType;

        [ShowIf( "IsProjectileType")]
        public ProjectileImpactType ProjectileImpactType;

        public SoundData AttackSfx;
        
        [ShowIf( "IsProjectileType")]
        public SoundData ProjectileSfx;
        
        public float PlayerDamageMultiplier = 1;
        public float EnemyDamageMultiplier = 1;

        public string FBConfigId;

        public bool IsWeaponEnableSfx;
        [ShowIf( "IsWeaponEnableSfx")]
        public SoundData WeaponEnableSfx;

        public bool IsSpecialAttackSfx;
        [ShowIf( "IsSpecialAttackSfx")]
        public SoundData SpecialAttackSfx;

        public bool IsAiming;
        [ShowIf( "IsAiming")]
        public AimingSettings AimingSettings;

        [ShowIf("IsPushType")]
        public float ImpulseStrength = 0;
        
        private bool IsPushType() => 
            WeaponType == WeaponType.PushMelle;
        private bool IsProjectileType() => 
            WeaponType == WeaponType.SimpleProjectile;
        
        private bool IsLaserWeaponType() => 
            WeaponType == WeaponType.LaserSaber ||
            WeaponType == WeaponType.LaserWhip;

        private bool IsSplash() => 
            ProjectileImpactType == ProjectileImpactType.Splash;
        
        [ShowIf( "IsLaserWeaponType")]
        public Color[] AllyLaserColors;
        
        [ShowIf( "IsLaserWeaponType")]
        public Color[] HostileLaserColors;

        public Color GetRandomWeaponColorFor(Faction faction)
        {
            Color[] sourceArray = faction switch
            {
                Faction.Player => AllyLaserColors,
                Faction.Enemy => HostileLaserColors,
                _ => null
            };

            if (sourceArray != null && sourceArray.Length > 0)
            {
                int index = UnityEngine.Random.Range(0, sourceArray.Length);
                return sourceArray[index];
            }

            return Color.white;
        }
        
        private IEnumerable<string> GetProjectileKeys()
        {
            return new List<string>
            {
                "-", "Stone", "Arrow", 
                "ThrowingAxe", "Mine", "JavelinMissile",
                "HimarsMissile", "Spear_1", "Cannonball", 
                "Laser_1", "Atlatl", "Projectile_1", "Projectile_2",
                "ScorpioArrow", "Fireball_0", "LaserBall_0", 
            };
        }

        private IEnumerable<string> GetMuzzleKeys()
        {
            return new List<string>
            {
                "-", "Muzzle_0", "Muzzle_1", "Muzzle_2",
                "Muzzle_3"
            };
        }

        private IEnumerable<string> GetExplosionKeys()
        {
            return new List<string>
            {
                "-", "Explosion_0", "Explosion_1", 
                "Explosion_2"
            };
        }
        
        public float GetProjectileDamageForFaction(Faction faction)
        {
            return faction switch
            {
                Faction.Enemy => Damage * EnemyDamageMultiplier,
                Faction.Player => Damage * PlayerDamageMultiplier,
                _ => throw new ArgumentOutOfRangeException($"Unsupported Faction: {faction}")
            };
        }
        
        public int GetLayerForFaction(Faction faction)
        {
            return faction switch
            {
                Faction.Enemy => Constants.Layer.ENEMY_PROJECTILE,
                Faction.Player => Constants.Layer.PLAYER_PROJECTILE,
                _ => throw new ArgumentOutOfRangeException($"Unsupported Race: {faction}")
            };
        }
        
        public int GetCollisionMask(Faction faction)
        {
            return GetLayerForFaction(faction) switch
            {
                Constants.Layer.PLAYER_PROJECTILE => 
                    (1 << Constants.Layer.MELEE_ENEMY) |
                    (1 << Constants.Layer.ENEMY_BASE) |
                    (1 << Constants.Layer.RANGE_ENEMY),

                Constants.Layer.ENEMY_PROJECTILE => 
                    (1 << Constants.Layer.MELEE_PLAYER) |
                    (1 << Constants.Layer.PLAYER_BASE) |
                    (1 << Constants.Layer.RANGE_PLAYER),

                _ => 0
            };
        }
        
        [Button("Auto Assign Mixers")]
        private void AutoAssignMixers()
        {
#if UNITY_EDITOR
            const string mixerPath = "Assets/_Game/Audio/Mixer.mixer";
            const string groupPath = "SFX";

            var mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>(mixerPath);
            if (mixer == null)
            {
                Debug.LogError($"Mixer not found at {mixerPath}");
                return;
            }

            var targetGroup = FindGroupByName(mixer, groupPath);
            if (targetGroup == null)
            {
                Debug.LogError($"Group '{groupPath}' not found in mixer.");
                return;
            }

            AssignGroup(AttackSfx, targetGroup);
            AssignGroup(ProjectileSfx, targetGroup);
            AssignGroup(WeaponEnableSfx, targetGroup);
            AssignGroup(SpecialAttackSfx, targetGroup);

            EditorUtility.SetDirty(this);
#endif
        }

#if UNITY_EDITOR
        private void AssignGroup(SoundData data, AudioMixerGroup targetGroup)
        {
            if (data == null) return;
            if (data.AudioMixerGroup != targetGroup)
            {
                data.AudioMixerGroup = targetGroup;
                EditorUtility.SetDirty(data.AudioMixerGroup);
            }
        }

        private AudioMixerGroup FindGroupByName(AudioMixer mixer, string groupName)
        {
            var groups = mixer.FindMatchingGroups(string.Empty);
            return groups.FirstOrDefault(g => g.name == groupName || g.name.EndsWith("/" + groupName));
        }
#endif
    }
}