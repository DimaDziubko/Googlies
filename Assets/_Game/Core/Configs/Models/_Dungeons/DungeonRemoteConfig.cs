using System.Collections.Generic;
using _Game.Core.UserState._State;

namespace _Game.Core.Configs.Models._Dungeons
{
    public class DungeonRemoteConfig
    {
        public int Id;
    
        public DungeonType Type;
        public int RequiredTimeline;
        public string Name;

        public float RecoveryTimeMinutes;
    
        public int KeysCount;
        public int VideosCount;

        public List<float> RewardLFC;
        
        public List<float> LightWarriorLFC;
        public List<float> MediumWarriorLFC;
        public List<float> HeavyWarriorLFC;
        public List<float> DifficultyLFC;
        
        public bool IsExpDifficulty;
        public List<float> ExpDifficultyEFC;
        public List<float> StartDifficulty;
        
        public int Delay;
        public int Cooldown;
        public float FoodProduction;
        public int InitialFoodAmount;
        
        public CurrencyType RewardType;

        public bool IsLocked;
        public string FBConfigId;
    }
}