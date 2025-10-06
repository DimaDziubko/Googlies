using System.Collections.Generic;
using _Game.Core.Configs.Models._Functions;
using _Game.Gameplay._DailyTasks.Scripts;

namespace _Game.Core.Configs.Models._DailyTaskConfig
{
    public class DailyTaskConfig
    {
        public int Id;
        public DailyTaskType DailyTaskType;
        public List<LinearFunction> LinearFunctions;
        public int DropChance;
        public string Description;
        public int Reward;
    }
}