using System.Collections.Generic;

namespace _Game.Core.Configs.Models._ScenarioConfig
{
    public class Wave
    {
        public int Id;
        public List<SpawnSequence> Sequences;

        public int GetTotalWarriorCount()
        {
            int totalCount = 0;

            foreach (var sequence in Sequences)
            {
                totalCount += sequence.Count;
            }

            return totalCount;
        }
    }
}