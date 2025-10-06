using System;
using System.Collections.Generic;
using _Game.Core.Configs.Models._Functions;
using _Game.Core.UserState._State;
using UnityEngine;

namespace _Game.Core.Configs.Models._Dungeons
{
    [CreateAssetMenu(fileName = "DungeonConfig", menuName = "Configs/Dungeon")]
    [Serializable]
    public class DungeonConfig : ScriptableObject
    {
        public int Id;
        public int RequiredTimeline;
        public DungeonType Type;
        public string Name;
        public int KeysCount;
        public int VideosCount;
        public Sprite Icon;
        public Sprite AdsIcon;
        public Sprite KeyIcon;
        public Sprite RewardIcon;
        public float RecoveryTimeMinutes;
        public string EnvironmentKey;
        public string AmbienceKey;
        
        public LinearFunction RewardFunction;
        
        public LinearFunction LightWarriorCountFunction;
        public LinearFunction MediumWarriorCountFunction;
        public LinearFunction HeavyWarriorCountFunction;
        
        public LinearFunction Difficulty;
        public Exponential ExpDifficulty;
        public List<float> StartDifficulty;

        public bool IsExponentionDifficultyUsed;
        
        public int Delay = 2;
        public int Cooldown = 3;
        public float FoodProduction = 0.18f;
        public int InitialFoodAmount = 0;
        
        public CurrencyType RewardType = CurrencyType.Gems;

        public bool IsLocked;
        public string FBConfigId;
    }
    
    
}