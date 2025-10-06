using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._Skills;
using _Game.Core.Configs.Repositories;
using _Game.Core.Services.Random;
using _Game.Core.UserState._State;
using UnityEngine;

namespace _Game.UI._Skills.Scripts
{
    public class SkillGenerator
    {
        private readonly ISkillConfigRepository _skillConfigRepository;
        private readonly IRandomService _random;
        private readonly IMyLogger _logger;
        private readonly ISkillsCollectionStateReadonly _skillsState;
        
        private Dictionary<int, int> _remainingSkills;
        public int TotalRemainingSkills => CalculateRemainingSkills().Values.Sum();
        
        public SkillGenerator(
            ISkillConfigRepository skillConfigRepository,
            IRandomService random, 
            IMyLogger logger, 
            ISkillsCollectionStateReadonly skillsState)
        {
            _skillConfigRepository = skillConfigRepository;
            _random = random;
            _logger = logger;
            _skillsState = skillsState;
        }
        
        public List<int> GenerateSkills(int amount)
        {   
            List<int> skillsId = new List<int>(amount);
            
            int initialDropListCount = _skillConfigRepository.InitialDropList.Count;

            _remainingSkills = CalculateRemainingSkills();

            int offset = 0;
            
            for (int i = 0; i < amount; i++)
            {
                int currentIndex = _skillsState.SkillsCollectedCount + offset;

                _logger.Log($"currentIndex {currentIndex}  initialDropListCount {initialDropListCount}");
                
                if (currentIndex < initialDropListCount)
                {
                    _logger.Log($"Get skill from initial drop list", DebugStatus.Info);
                    int skillId = _skillConfigRepository.InitialDropList[currentIndex];
                    skillsId.Add(skillId);
                    _remainingSkills[skillId]--;

                    offset++;
                }
                else
                {
                    int id = SelectIdWithDropChance();
                    
                    if (_skillConfigRepository.TryGetSkill(id, out var skillConfig))
                    {
                        skillsId.Add(id);
                    }
                    
                    _remainingSkills[id]--;
                }
            }
            
            foreach (var id in skillsId)
            {
                _logger.Log($"GENERATED SKILL WITH ID: {id}", DebugStatus.Info);
            }
            
            return skillsId;
        }
        
        private Dictionary<int, int> CalculateRemainingSkills()
        {
            Dictionary<int, int> remainingSkills = new Dictionary<int, int>();
            
            foreach (var skill in _skillConfigRepository.GetAllSkills())
            {
                remainingSkills.Add(skill.Id, skill.MaxCount());
            }
            
            foreach (var skill in _skillsState.ActiveSkills)
            {
                int collected = _skillConfigRepository.ForSkill(skill.Id).GetAccumulatedCountForLevel(skill.Level) + skill.Count;
                
                if (remainingSkills.ContainsKey(skill.Id))
                {
                    remainingSkills[skill.Id] -= collected;
                }
            }
            
            return remainingSkills;
        }

        private int SelectIdWithDropChance()
        {
            List<SkillConfig> skillConfigs = _skillConfigRepository.GetAllSkills();
            
            if (skillConfigs == null || skillConfigs.Count == 0)
            {
                _logger.Log("No skills found in the repository!", DebugStatus.Fault);
                return -1;
            }

            List<int> availableToDropSkillIds = _remainingSkills
                .Where(x => x.Value > 0)
                .Select(x => x.Key)  
                .ToList();
            
            List<(int skillId, int cumulativeChance) > weightedList = new();
            
            int cumulativeChance = 0;

            foreach (var id in availableToDropSkillIds)
            {
                var skillConfig = _skillConfigRepository.ForSkill(id);
                cumulativeChance += skillConfig.DropChance;
                weightedList.Add((id, cumulativeChance));
            }
            
            int randomValue = _random.Next(0, cumulativeChance);
            
            foreach (var (skillId, chanceThreshold) in weightedList)
            {
                if (randomValue < chanceThreshold)
                {
                    return skillId;
                }
            }
            
            _logger.Log("Random selection failed, returning fallback skill ID", DebugStatus.Warning);
            
            return skillConfigs[0].Id;
        }
    }
}