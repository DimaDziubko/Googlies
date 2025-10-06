using System.Collections.Generic;

namespace _Game.Core.Configs.Models._DailyTaskConfig
{
    public class GeneralDailyTaskConfig
    {
        public int Id;
        public int MaxCountPerDay;
        public int RecoverTimeMinutes;
        public List<DailyTaskConfig> DailyTaskConfigs;
    }
}