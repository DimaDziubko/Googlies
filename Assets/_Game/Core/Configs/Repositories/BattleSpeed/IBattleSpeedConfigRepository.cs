using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Models._BattleSpeedConfig;

namespace _Game.Core.Configs.Repositories.BattleSpeed
{
    public interface IBattleSpeedConfigRepository
    {
        List<BattleSpeedConfig> GetBattleSpeedConfigs();
        BattleSpeedConfig GetBattleSpeedConfig(int battleSpeedId);
    }
}