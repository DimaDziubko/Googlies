using System.Collections.Generic;
using _Game.Core._Reward;

namespace _Game.Core.UserState._State
{
    public class RewardItemWithObjectiveCollection
    {
        public int Id;
        public List<RewardItemWithObjective> WeeklyProgressRewards;
    }
}