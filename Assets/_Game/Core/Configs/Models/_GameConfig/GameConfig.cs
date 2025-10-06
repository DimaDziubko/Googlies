using System.Collections.Generic;
using _Game.Core.Configs.Models._AdsConfig;
using _Game.Core.Configs.Models._AgeConfig;
using _Game.Core.Configs.Models._BattleConfig;
using _Game.Core.Configs.Models._BattleSpeedConfig;
using _Game.Core.Configs.Models._Cards;
using _Game.Core.Configs.Models._DailyTaskConfig;
using _Game.Core.Configs.Models._DifficultyConfig;
using _Game.Core.Configs.Models._Dungeons;
using _Game.Core.Configs.Models._FoodBoostConfig;
using _Game.Core.Configs.Models._IconConfig;
using _Game.Core.Configs.Models._ShopConfig;
using _Game.Core.Configs.Models._Skills;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Configs.Models._WeaponConfig;
using _Game.UI._CardsGeneral._Cards.Scripts;

namespace _Game.Core.Configs.Models._GameConfig
{
    public class GameConfig
    {
        public Dictionary<int, WarriorConfig> WarriorConfigs;
        public Dictionary<int, WeaponConfig> WeaponConfigs;
        public Dictionary<int, AgeConfig> AgeConfigs;
        public Dictionary<int, BattleConfig> BattleConfigs;

        public IconConfig IconConfig;
        public FoodBoostConfig FoodBoostConfig;
        public List<BattleSpeedConfig> BattleSpeedConfigs;
        public ShopConfig ShopConfig;
        public AdsConfig AdsConfig;
        public GeneralDailyTaskConfig GeneralDailyTaskConfig;
        public Dictionary<CardType, List<CardConfig>> CardConfigsByType;
        public Dictionary<int, CardConfig> CardConfigsById;
        public CardsPricingConfig CardPricingConfig;
        public DifficultyConfig DifficultyConfig;

        public SummoningData SummoningData;

        public Dictionary<int, DungeonConfig> Dungeons;
        
        public FeatureSettings FeatureSettings;
        
        public Dictionary<int, SkillConfig> SkillConfigs;
        public SkillExtraConfig SkillExtraConfig;
    }
}