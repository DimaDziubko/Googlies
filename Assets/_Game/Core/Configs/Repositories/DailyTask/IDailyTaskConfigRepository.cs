using System.Collections.Generic;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Models._DailyTaskConfig;

namespace _Game.Core.Configs.Repositories.DailyTask
{
    public interface IDailyTaskConfigRepository
    {
        int MaxDailyCountPerDay { get; }
        public float RecoverTimeMinutes { get; }
        List<DailyTaskConfig> GetDailyTaskConfigs();
    }
}