using Pathfinding.RVO;
using UnityEngine;

namespace _Game.Utils
{
    public sealed class Constants
    {
        public sealed class ItemId
        {
            public const int COINS = 690;
            public const int GEMS = 691;
            public const int SKILL_POTION = 692;
            
            public const int RED_KEY = 696;
            public const int YELLOW_KEY = 697;
            public const int GREEN_KEY = 698;
            
            public const int CARD = 700;
        }
        
        public sealed class LocalConfigPath
        {
            public const string GAME_LOCAL_CONFIG_CONTAINER_PATH = "GameLocalConfigContainer";
            public const string GENERAL_WARRIOR_CONFIG_PATH = "Warrior/GeneralWarriorsConfig";
            public const string GENERAL_WEAPON_CONFIG_PATH = "Weapon/GeneralWeaponConfig";
            public const string COMMON_CONFIG_PATH = "Common/CommonConfig";
            public const string GENERAL_AGE_CONFIG_PATH = "Age/GeneralAgesConfig";
            public const string GENERAL_BATTLE_CONFIG_PATH = "Battle/GeneralBattlesConfig";
            public const string CARDS_CONFIG_PATH = "Card/CardsConfig";
            public const string SUMMONING_CONFIG_PATH = "Card/SummoningConfig";
            public const string CARDS_PRICING_PATH = "Card/CardsPricingConfig";
            public const string DIFFICULTY_PATH = "Difficulty/DifficultyConfig";
            
            public const string DUNGEONS_PATH = "Dungeons/DungeonsConfig";
        }
        
        public sealed class ColorTheme
        {
            public static Color THEMED_PROGRESS_BAR = Color.cyan;
            public static Color THEMED_PROGRESS_BAR_FULL = Color.green;
        }
        
        public sealed class Money
        {
            public const int MIN_COINS_PER_BATTLE = 9;
        }
        
        public sealed class Config
        {
            public const int AGES_IN_TIMELINE = 6;
        }
        
        public sealed class Scenes
        {
            public const string UI = "UI";
            public const string ZOMBIE_RUSH_MODE = "ZombieRushMode";
            public const string STARTUP = "Startup";
            public const string BATTLE_MODE = "BattleMode";
        }

        public sealed class SortingLayer
        {
            public const float SORTING_TRESHOLD = 0.1f;
            public const int SORTING_ORDER_MIN = -32768;
            public const int SORTING_ORDER_MAX = 32767;
        }

        public sealed class SpecialColors
        {
            public const string SPECIAL_GREY = "#B2B2B2";
            public const string SPECIAL_BLUE = "#87A4E6";
        }

        public sealed class Layer
        {
            public const int PLAYER_PROJECTILE = 16;
            public const int ENEMY_PROJECTILE = 17;
            public const int MELEE_PLAYER = 8;
            public const int MELEE_ENEMY = 9;
            public const int ENEMY_BASE = 14;
            public const int PLAYER_BASE = 15;
            public const int PLAYER_AGGRO = 10;
            public const int ENEMY_AGGRO = 11;
            public const int PLAYER_ATTACK = 12;
            public const int ENEMY_ATTACK = 13;
            public const int RANGE_PLAYER = 19;
            public const int RANGE_ENEMY = 20;
            public const int SOLO_PLAYER = 21;

            public const RVOLayer RVO_MELEE_ENEMY = RVOLayer.Layer3;
            public const RVOLayer RVO_RANGE_ENEMY = RVOLayer.Layer4;

            public const RVOLayer RVO_MELEE_PLAYER = RVOLayer.Layer5;
            public const RVOLayer RVO_RANGE_PLAYER = RVOLayer.Layer2;
            
            public const RVOLayer RVO_SOLO_PLAYER = RVOLayer.Layer6;
        }

        public sealed class ComparisonThreshold
        {
            public const float EPSILON = 0.01f;
            public const float MONEY_EPSILON = 0.01f;
            public const float UNIT_ROTATION_EPSILON = 0.05f;
        }

        public sealed class TutorialSteps
        {
            public const int START_TUTORIAL_KEY = -1;
            
            public const int BUILDER = 1;
            public const int UPGRADE_SCREEN_BTN = 2;
            public const int FOOD_UPGRADE = 3;
            public const int EVOLVE_HINT = 4;
            public const int EVOLVE = 5;
            public const int DAILY = 6;
            public const int CARDS_SCREEN_BTN = 7;
            public const int CARDS_PURCHASE = 8;
            public const int SKILLS_SCREEN_BTN = 9;
            public const int SKILLS_TAB = 10;
            
            public const int LAST_STEP = 10;
            public const int STEPS_COUNT_WITH_START_KEY = 11;
            
            public const int COMPLETE_TUTORIAL_KEY = -2;
        }
        
        public sealed class ConfigKeys
        {
            //Skills
            public const string SKILL_EXTRA_CONFIG = "SkillExtraConfig";
            
            public const string FEATURE_SETTINGS = "FeatureSettings";
            public const string WEAPONS = "Weapons";
            public const string WARRIORS = "Warriors";
            public const string AGES = "Ages";
            public const string BATTLES = "Battles";

            //Summoning
            public const string SUMMONING_CONFIGS = "SummoningConfigs";
            
            //Ads
            public const string ADS_CONFIG = "AdsConfig";

            //FreeGemsPackDayConfig
            public const string FREE_GEMS_PACK_DAY_CONFIG = "FreeGemsPackDayConfig";

            //BattleSpeed
            public const string BATTLE_SPEEDS_CONFIGS = "BattleSpeedConfigs";

            //FoodBoost
            public const string FOOD_BOOST_CONFIG = "FoodBoostConfig";

            //Common
            public const string COMMON_CONFIG = "CommonConfig";
            public const string MISSING_KEY = "-";

            //Timeline common
            public const string ID = "Id";

            //Timeline
            public const string TIMELINES = "Timelines";

            //Shop
            public const string SHOP_CONFIG = "ShopConfig";
            public const string PLACEMENT = "adPlacement";
            
            //GeneralDailyTask
            public const string GENERAL_DAILY_TASK_CONFIG = "GeneralDailyTaskConfig";
        }
        
        public static class FeatureThresholds
        {
            public const int BATTLE_SPEED_BATTLE_THRESHOLD = 10;
            public const int FOOD_BOOST_BATTLE_THRESHOLD = 2;
            public const int PAUSE_BATTLE_THRESHOLD = 1;
            public const int UPGRADES_SCREEN_BATTLE_THRESHOLD = 1;
            public const int EVOLUTION_SCREEN_BATTLE_THRESHOLD = 3;
            public const int X2_BATTLE_THRESHOLD = 2;
            public const int SHOP_BATTLE_THRESHOLD = 6;
            public const int DAILY_BATTLE_THRESHOLD = 5;
            public const int DUNGEONS_TIMELINE_THRESHOLD = 1;
        }
        
        public class StreamingAssets
        {
            public static string EMBEDDED_SKILLS_CONFIG = "skillsConfig.json" ;
            public static string EMBEDDED_WARRIORS_CONFIG = "warriorsConfig.json";
            public static string EMBEDDED_BATTLES_CONFIG = "battlesConfig.json";
            public static string EMBEDDED_AGES_CONFIG = "agesConfig.json";
            public static string EMBEDDED_WEAPONS_CONFIG = "weaponsConfig.json";
            public static string EMBEDDED_DUNGEON_CONFIG = "dungeonConfig.json";
            public static string EMBEDDED_EXTRA_CONFIG = "extraConfig.json";
        }
    }
}