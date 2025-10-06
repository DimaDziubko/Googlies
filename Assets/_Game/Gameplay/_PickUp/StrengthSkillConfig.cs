using _Game.Core.Configs.Models._Skills;
using UnityEngine;

namespace _Game.Gameplay._PickUp
{
    [CreateAssetMenu(fileName = "StrengthSkillConfig", menuName = "Configs/Skill/Strength")]
    public class StrengthSkillConfig : SkillConfig
    {
        public float InitialBuffValue = 0.5f;
        public float InreasePerLevel = 0.08f;
        public Vector3 IncreaseScale = new(1.2f, 1.2f, 1.2f);
        public override string GetDescription(int level, bool needShowBoost)
        {
            if (needShowBoost)
            {
                return $"The next unit you spawn is bigger, increasing Unit Damage and Unit Health by {GetValue(level)*100}% <style=Green>(+{GetDelta(level)})%</style>";
            }
            
            return $"The next unit you spawn is bigger, increasing Unit Damage and Unit Health by {GetValue(level)*100}%";
        }

        public override float GetValue(int level) => 
            InitialBuffValue + ((level - 1) * InreasePerLevel);
    }
}