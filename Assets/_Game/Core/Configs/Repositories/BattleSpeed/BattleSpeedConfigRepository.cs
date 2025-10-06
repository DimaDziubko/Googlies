using System.Collections.Generic;
using _Game.Core.Configs.Models._BattleSpeedConfig;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.Configs.Repositories.BattleSpeed
{
    public class BattleSpeedConfigRepository : IBattleSpeedConfigRepository
    {
        private readonly IUserContainer _persistentData;

        public BattleSpeedConfigRepository(
            IUserContainer persistentData)
        {
            _persistentData = persistentData;
        }
        public List<BattleSpeedConfig> GetBattleSpeedConfigs() => 
            _persistentData.GameConfig.BattleSpeedConfigs;

        public BattleSpeedConfig GetBattleSpeedConfig(int battleSpeedId) => 
            _persistentData.GameConfig.BattleSpeedConfigs[battleSpeedId];
    }
}