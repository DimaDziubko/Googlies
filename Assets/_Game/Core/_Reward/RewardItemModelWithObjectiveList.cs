using System.Collections.Generic;

namespace _Game.Core._Reward
{
    public class RewardItemModelWithObjectiveList
    {
        public int Id;
        public IReadOnlyList<RewardItemModelWithObjective> RewardItems;
    }
}