using _Game.Utils.Extensions;
using UnityEngine;

namespace _Game.Core.Configs.Models._Skills
{
    [CreateAssetMenu(fileName = "MightyPushSkillConfig", menuName = "Configs/Skill/MightyPush")]
    public class MightyPushSkillConfig : SkillConfig
    {
        public float InitialPushPercentage = 0.7f;
        public float IncreasePercentagePerLevel = 0.07f;
        public float ImpulseStrength = 5f;

        public override string GetDescription(int level, bool needShowBoost)
        {
            if (needShowBoost)
            {
                return $"Push back all enemy units, dealing {(GetValue(level) * 100).ToCompactFormat()}%<style=Green>(+{GetDelta(level)})%</style> of Starting Unit Damage";
            }
            return
                $"Push back all enemy units, dealing {(GetValue(level) * 100).ToCompactFormat()}% of Starting Unit Damage";
        }

        public override float GetValue(int level) => 
            InitialPushPercentage  + IncreasePercentagePerLevel * (level - 1);
        
    }
}