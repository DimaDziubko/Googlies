using System;
using _Game.Core.Configs.Models._AgeConfig;
using _Game.Core.Configs.Models._BattleConfig;
using _Game.Core.Configs.Models._Cards;
using _Game.Core.Configs.Models._DailyTaskConfig;
using _Game.Core.Configs.Models._DifficultyConfig;
using _Game.Core.Configs.Models._Dungeons;
using _Game.Core.Configs.Models._IconConfig;
using _Game.Core.Configs.Models._Skills;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Configs.Models._WeaponConfig;
using UnityEngine;

namespace _Game.Core.Configs.Models
{
    [CreateAssetMenu(fileName = "GameLocalConfigContainer", menuName = "Configs/GameLocalConfigContainer")]
    [Serializable]
    public class GameLocalConfigContainer : ScriptableObject, IConfigWithId
    {
        public int Id;
        
        public GeneralSkillsConfig GeneralSkillsConfig;
        public DungeonsConfig DungeonsConfig;
        public GeneralDailyTaskConfig GeneralDailyTaskConfig;
        public GeneralWarriorsConfig GeneralWarriorsConfig;
        public GeneralWeaponConfig GeneralWeaponConfig;
        public DifficultyConfig DifficultyConfig;
        public IconConfig IconConfig;
        public GeneralAgesConfig GeneralAgesConfig;
        public GeneralBattlesConfig GeneralBattleConfig;
        public CardsConfig CardsConfig;
        public CardsPricingConfig CardsPricingConfig;
        public SummoningConfigs SummoningConfigs;
        
        int IConfigWithId.Id => Id;
    }
}