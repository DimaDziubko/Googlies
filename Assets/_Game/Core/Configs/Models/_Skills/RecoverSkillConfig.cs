using System;
using System.Collections.Generic;
using _Game.Utils.Extensions;
using UnityEngine;

namespace _Game.Core.Configs.Models._Skills
{
    [CreateAssetMenu(fileName = "RecoverSkillConfig", menuName = "Configs/Skill/Recover")]
    public class RecoverSkillConfig : SkillConfig
    {
        public float InitialRecoverPercentage = 1.32f;
        public List<float> IncreaseList  = new() {0.03f, 0.02f};
        public override string GetDescription(int level, bool needShowBoost)
        {
            if (needShowBoost)
               return  $"Recover all of your units from the battlefield and get {(GetValue(level) * 100).ToCompactFormat()}% <style=Green>(+{GetDelta(level)})%</style> of their Food Cost refunded";
            return
                $"Recover all of your units from the battlefield and get {(GetValue(level) * 100).ToCompactFormat()}% of their Food Cost refunded";
        }
        
        public override float GetValue(int level)
        {
            if (level < 1)
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be at least 1.");
            
            if (level == 1)
                return InitialRecoverPercentage;
            
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
            
            return InitialRecoverPercentage + completeCycles * totalCycleSum + partialSum;
        }
    }
}