using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._BattleConfig;
using _Game.Utils;
using Newtonsoft.Json.Linq;

namespace _Game.Core.Loading
{
    public class BattlesConfigMapper : BaseConfigMapper
    {
        public BattlesConfigMapper(IMyLogger logger) : base(logger)
        {
            
        }
        
        public Dictionary<int, BattleConfig> LoadAndMergeBattleConfigs(JObject jsonData, GeneralBattlesConfig localBattlesConfig)
        {
            _logger.Log("Extracting battle configs", DebugStatus.Warning);

            List<RemoteBattleConfig> remoteBattlesData =
                ParseConfigList<RemoteBattleConfig>(jsonData, Constants.ConfigKeys.BATTLES);
            
            Dictionary<int, BattleConfig> localBattleConfigs = localBattlesConfig
                .BattleConfigs
                .ToDictionary(x => x.Id);

            MergeBattleConfigs(remoteBattlesData, localBattleConfigs);
            return localBattleConfigs;
        }

        private void MergeBattleConfigs(List<RemoteBattleConfig> remoteBattlesData,
            Dictionary<int, BattleConfig> localBattleConfigs)
        {
            foreach (var remote in remoteBattlesData)
            {
                if (localBattleConfigs.TryGetValue(remote.Id, out var local))
                {
                    local.Scenario = remote.Scenario;
                    local.WarriorsId = remote.WarriorsId;
                    local.CoinsPerBase = remote.CoinsPerBase;
                    local.MaxCoinsPerBattle = remote.MaxCoinsPerBattle;
                    local.EnemyBaseHealth = remote.EnemyBaseHealth;
                    local.FBConfigId = remote.FBConfigId;
                }
            }
        }
    }
}