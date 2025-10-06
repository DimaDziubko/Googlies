using System;
using _Game.Utils.Extensions;
using UnityEngine;

namespace _Game.Core.Configs.Models._Skills
{
    [CreateAssetMenu(fileName = "KaboomSkillConfig", menuName = "Configs/Skill/Kaboom")]
    public class KaboomSkillConfig : SkillConfig
    {
        public float InitialValue = 0.3f;
        public float PersentPerLevel = 0.14f; 
        public float WaveRadius = 0.5f;
        public float EffectLifetime = 5f;
        
        public override string GetDescription(int level, bool needShowBoost)
        {
            if (needShowBoost)
            {
                return $"All of your units on the battlefield release a shockwave, dealing {(GetValue(level) * 100).ToCompactFormat(100)}% <style=Green>(+{GetDelta(level)})%</style> of their Damage";
            }
            return
                $"All of your units on the battlefield release a shockwave, dealing {(GetValue(level) * 100).ToCompactFormat(999)}% of their Damage";
        }

        public override float GetValue(int level)
        {
            if (level < 1)
               throw new ArgumentOutOfRangeException(nameof(level), "Level must be at least 1.");
            return InitialValue + PersentPerLevel * (level - 1);
        }

        public override float GetLifetime() => EffectLifetime;
    }
}