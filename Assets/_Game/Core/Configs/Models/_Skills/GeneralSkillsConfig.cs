using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Core.Configs.Models._Skills
{
    [CreateAssetMenu(fileName = "GeneralSkillConfig", menuName = "Configs/GeneralSkillConfigs")]
    public class GeneralSkillsConfig : ScriptableObject
    {
        public int Id;
        [FormerlySerializedAs("SkillPricingConfig")] public SkillExtraConfig SkillsExtraConfig;
        public List<SkillConfig> SkillConfigs;
    }
}