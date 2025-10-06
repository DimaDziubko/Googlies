using System;
using System.Collections.Generic;
using _Game.Core.Boosts;
using _Game.Core.UserState._State;

namespace _Game.Core.Configs.Models._Skills
{
    [Serializable]
    public class SkillExtraConfig
    {
        public int Id;
        
        public int RequiredTimeline = 2;
        public List<int> InitialDropList;
        public List<SkillSlotConfig> SkillSlotConfigs;
        
        public int x1SkillPrice = 8;
        public int x10SkillPrice = 76;
        
        public CurrencyData RewardPerEvolve => new()
        {
            Amount = 8,
            Source = ItemSource.Evolve,
            Type = CurrencyType.SkillPotion,
        };

        public CurrencyData RewardPerTravel => new()
        {
            Amount = 8,
            Source = ItemSource.Evolve,
            Type = CurrencyType.SkillPotion,
        };
    }

    [Serializable]
    public class SkillSlotConfig
    {
        public int Id;
        public int RequiredTimeline;
    }
}