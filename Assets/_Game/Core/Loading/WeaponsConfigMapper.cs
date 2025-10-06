using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._WeaponConfig;
using _Game.Utils;
using Newtonsoft.Json.Linq;

namespace _Game.Core.Loading
{
    public class WeaponsConfigMapper : BaseConfigMapper
    {
        public WeaponsConfigMapper(IMyLogger logger) : base(logger)
        {
        }
        
        public Dictionary<int, WeaponConfig> LoadAndMergeWeaponConfigs(JObject jsonData, GeneralWeaponConfig localWeaponConfig)
        {
            List<RemoteWeaponConfig> remoteWeapons =
                ParseConfigList<RemoteWeaponConfig>(jsonData, Constants.ConfigKeys.WEAPONS);

            Dictionary<int, WeaponConfig> weapons =
                localWeaponConfig
                    .Weapons
                    .ToDictionary(x => x.Id);
            MergeWeaponConfigs(remoteWeapons, weapons);
            return weapons;
        }

        private void MergeWeaponConfigs(List<RemoteWeaponConfig> remoteWeapons, Dictionary<int, WeaponConfig> weapons)
        {
            foreach (var remoteWeapon in remoteWeapons)
            {
                if (weapons.TryGetValue(remoteWeapon.Id, out var localWeapon))
                {
                    localWeapon.Damage = remoteWeapon.Damage;
                    localWeapon.ProjectileSpeed = remoteWeapon.ProjectileSpeed;
                    localWeapon.SplashRadius = remoteWeapon.SplashRadius;
                    localWeapon.EnemyDamageMultiplier = remoteWeapon.EnemyDamageMultiplier;
                    localWeapon.PlayerDamageMultiplier = remoteWeapon.PlayerDamageMultiplier;
                    localWeapon.TrajectoryWarpFactor = remoteWeapon.TrajectoryWarpFactor;
                    localWeapon.SplashDamageRatio = remoteWeapon.SplashDamageRatio;
                    localWeapon.FBConfigId = remoteWeapon.FBConfigId;
                    localWeapon.ProjectileImpactType = remoteWeapon.ProjectileImpactType;
                    localWeapon.trajectoryType = remoteWeapon.trajectoryType;
                }
            }
        }
    }
}