using System;
using System.Collections.Generic;
using _Game.Utils.Extensions;
using UnityEngine;

namespace _Game.Core.Configs.Models._Skills
{
    [CreateAssetMenu(fileName = "HornSkillConfig", menuName = "Configs/Skill/Horn")]
    public class HornSkillConfig : SkillConfig
    {
        public float InitialBuffPercentage = 0.06f;
        public List<float> IncreaseList  = new() {0.01f, 0.01f, 0.015f};
        
        public override string GetDescription(int level, bool needShowBoost)
        {
            if (needShowBoost)
            {
                return $"Improve the morale of your units on the battlefield, increasing Unit Damage and Unit Health by {(GetValue(level) * 100).ToCompactFormat(100)}% <style=Green>(+{GetDelta(level)})%</style>";
            }
            return
                $"Improve the morale of your units on the battlefield, increasing Unit Damage and Unit Health by {(GetValue(level) * 100).ToCompactFormat(100)}%";
        }

        public override float GetValue(int level)
        {
            if (level < 1)
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be at least 1.");
            
            if (level == 1)
                return InitialBuffPercentage;
            
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
            
            return InitialBuffPercentage + completeCycles * totalCycleSum + partialSum;
        }
    }
}