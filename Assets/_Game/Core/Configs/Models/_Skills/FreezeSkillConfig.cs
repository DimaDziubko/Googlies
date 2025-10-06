using _Game.Utils.Extensions;
using UnityEngine;

namespace _Game.Core.Configs.Models._Skills
{
    [CreateAssetMenu(fileName = "FreezeSkillConfig", menuName = "Configs/Skill/Freeze")]
    public class FreezeSkillConfig : SkillConfig
    {
        public float FreezeDurationPerLevel = 0.5f;
        public override string GetDescription(int level, bool needShowBoost)
        {
            if(!needShowBoost)
                return $"Freeze all enemy for {(FreezeDurationPerLevel * level).ToCompactFormat()}s";
            
            return $"Freeze all enemy for {(FreezeDurationPerLevel * level).ToCompactFormat()}s <style=Green>(+{GetDelta(level)})s</style>";
        }
        
        public override float GetValue(int level) => 
            FreezeDurationPerLevel * level;
        
        public override float GetDuration(int level) => 
            FreezeDurationPerLevel * level;

        protected override string GetDelta(int currentLevel)
        {
            int nextLevel = currentLevel + 1;
            var delta = GetValue(nextLevel) - GetValue(currentLevel);
            string deltaString = delta.ToCompactFormat();
            return deltaString;
        }
    }
}