using System;
using System.Collections.Generic;
using _Game.Utils.Extensions;
using UnityEngine;

namespace _Game.Core.Configs.Models._Skills
{
    [CreateAssetMenu(fileName = "MeteorsSkillConfig", menuName = "Configs/Skill/Meteors")]
    public class MeteorsSkillConfig : SkillConfig
    {
        public int MeteorsAmount = 20;
        public MeteorConfig MeteorConfig;
        public float InitialDamagePercentage = 0.3f;
        public List<float> IncreaseList = new() {0.04f, 0.03f};
        public float SpawnDelay = 2f;

        public override string GetDescription(int level, bool needShowBoost)
        {
            if(needShowBoost)
                return $"Bring down a shower of meteorites, each dealing {(GetValue(level) * 100).ToCompactFormat()}% <style=Green>(+{GetDelta(level)}%</style> of Starting Unit Damage";
            return 
                $"Bring down a shower of meteorites, each dealing {(GetValue(level) * 100).ToCompactFormat()}% of Starting Unit Damage";
        }
        
        public override float GetValue(int level)
        {
            if (level < 1)
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be at least 1.");
            
            if (level == 1)
                return InitialDamagePercentage;
            
            int incrementsToAdd = level - 1;
            int listLength = IncreaseList.Count;
            
            int completeCycles = incrementsToAdd / listLength;
            int remainder = incrementsToAdd % listLength;
            
            float totalCycleSum = 0;
            foreach (float inc in IncreaseList)
            {
                totalCycleSum += inc;
            }
            
            float partialSum = 0;
            for (int i = 0; i < remainder; i++)
            {
                partialSum += IncreaseList[i];
            }
            
            return InitialDamagePercentage + completeCycles * totalCycleSum + partialSum;
        }
    }
}