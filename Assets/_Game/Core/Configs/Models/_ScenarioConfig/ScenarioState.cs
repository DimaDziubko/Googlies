using System.Collections.Generic;

namespace _Game.Core.Configs.Models._ScenarioConfig
{
    public class ScenarioState
    {
        public int Id;
        public List<Wave> Waves;

        public int GetTotalCount()
        {
            int totalCount = 0;

            foreach (var wave in Waves)
            {
                totalCount += wave.GetTotalWarriorCount();
            }

            return totalCount;
        }
    }
}