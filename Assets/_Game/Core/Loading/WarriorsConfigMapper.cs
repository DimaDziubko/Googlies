using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Utils;
using Newtonsoft.Json.Linq;

namespace _Game.Core.Loading
{
    public class WarriorsConfigMapper : BaseConfigMapper
    {
        public WarriorsConfigMapper(IMyLogger logger) : base(logger)
        {
        }
        
        public Dictionary<int, WarriorConfig> LoadAndMergeWarriorConfigs(JObject jsonData, GeneralWarriorsConfig localWarriorsConfig)
        {
            List<RemoteWarriorConfig> remoteWarriorsData =
                ParseConfigList<RemoteWarriorConfig>(jsonData, Constants.ConfigKeys.WARRIORS);
            Dictionary<int, WarriorConfig> warriorConfigs =
                localWarriorsConfig
                    .WarriorConfigs
                    .ToDictionary(x => x.Id);
            MergeWarriorConfigs(remoteWarriorsData, warriorConfigs);
            return warriorConfigs;
        }

        private void MergeWarriorConfigs(List<RemoteWarriorConfig> remoteWarriorsData,
            Dictionary<int, WarriorConfig> warriorConfigs)
        {
            foreach (var remoteWarrior in remoteWarriorsData)
            {
                if (warriorConfigs.TryGetValue(remoteWarrior.Id, out var localWarrior))
                {
                    localWarrior.Health = remoteWarrior.Health;
                    localWarrior.Price = remoteWarrior.Price;
                    localWarrior.FoodPrice = remoteWarrior.FoodPrice;
                    localWarrior.Speed = remoteWarrior.Speed;
                    localWarrior.AttackPerSecond = remoteWarrior.AttackPerSecond;
                    localWarrior.AttackDistance = remoteWarrior.AttackDistance;
                    localWarrior.CoinsPerKill = remoteWarrior.CoinsPerKill;
                    localWarrior.EnemyHealthMultiplier = remoteWarrior.EnemyHealthMultiplier;
                    localWarrior.PlayerHealthMultiplier = remoteWarrior.PlayerHealthMultiplier;
                    localWarrior.FBConfigId = remoteWarrior.FBConfigId;
                }
            }
        }
    }
}