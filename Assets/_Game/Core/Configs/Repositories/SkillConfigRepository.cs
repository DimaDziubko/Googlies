using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.Core.Configs.Models._Skills;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.Configs.Repositories
{
    public interface ISkillConfigRepository
    {
        int GetAllSkillsCount();
        SkillConfig ForSkill(int id);
        int GetX1SkillPrice();
        int GetX10SkillPrice();
        List<int> InitialDropList { get;}
        CurrencyData RewardPerEvolve { get;}
        CurrencyData RewardPerTravel { get;}
        bool TryGetSkill(int id, out SkillConfig skillConfig);
        List<SkillConfig> GetAllSkills();
        int TimelineThresholdForSlot(int slotId);
    }
    public class SkillConfigRepository : ISkillConfigRepository  
    {
        private readonly IUserContainer _userContainer;
        private readonly IMyLogger _logger;

        public SkillConfigRepository(
            IUserContainer userContainer,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _logger = logger;
        }

        public int GetAllSkillsCount() => 
            _userContainer.GameConfig.SkillConfigs.Count;

        public SkillConfig ForSkill(int id) => 
            _userContainer.GameConfig.SkillConfigs[id];

        public int GetX1SkillPrice() => 
            _userContainer.GameConfig.SkillExtraConfig.x1SkillPrice;

        public int GetX10SkillPrice() => 
            _userContainer.GameConfig.SkillExtraConfig.x10SkillPrice;

        public List<int> InitialDropList => _userContainer.GameConfig.SkillExtraConfig.InitialDropList;
        public CurrencyData RewardPerEvolve  => _userContainer.GameConfig.SkillExtraConfig.RewardPerEvolve;
        public CurrencyData RewardPerTravel => _userContainer.GameConfig.SkillExtraConfig.RewardPerTravel;
        
        bool ISkillConfigRepository.TryGetSkill(int id, out SkillConfig skillConfig) => 
            _userContainer.GameConfig.SkillConfigs.TryGetValue(id, out skillConfig);

        public List<SkillConfig> GetAllSkills() => 
            _userContainer.GameConfig.SkillConfigs.Values.ToList();

        public int TimelineThresholdForSlot(int slotId)
        {
            var gameConfig = _userContainer?.GameConfig?.SkillExtraConfig;
    
            if (gameConfig == null || gameConfig.SkillSlotConfigs == null)
            {
                _logger?.LogError("GameConfig or SkillSlotConfigs is null.");
                return int.MaxValue;
            }

            var slotConfig = gameConfig.SkillSlotConfigs.FirstOrDefault(x => x.Id == slotId);
    
            if (slotConfig == null)
            {
                _logger?.LogWarning($"Skill slot with ID {slotId} not found.");
                return int.MaxValue;
            }

            return slotConfig.RequiredTimeline;
        }
    }
}