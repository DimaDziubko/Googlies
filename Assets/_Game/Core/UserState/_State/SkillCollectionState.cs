using System;
using System.Collections.Generic;
using System.Linq;

namespace _Game.Core.UserState._State
{
    public interface ISkillsCollectionStateReadonly
    {
        event Action<List<ActiveSkill>> SkillsCollected;
        event Action<ActiveSkill> SkillRemoved;
        List<ActiveSkill> ActiveSkills { get; }
        int SkillsCollectedCount { get; }
    }
    public class SkillCollectionState : ISkillsCollectionStateReadonly
    {
        public event Action<List<ActiveSkill>> SkillsCollected;
        public event Action<ActiveSkill> SkillRemoved;
        public event Action SkillsCollectionChanged;
        
        public int SkillsCollectedCount;
        public List<ActiveSkill> ActiveSkills;
        
        List<ActiveSkill> ISkillsCollectionStateReadonly.ActiveSkills => ActiveSkills;
        int ISkillsCollectionStateReadonly.SkillsCollectedCount => SkillsCollectedCount;
        
        public void AddSkills(List<int> skillsId)
        {
            SkillsCollectedCount += skillsId.Count;
            
            var skillDictionary = ActiveSkills.ToDictionary(skill => skill.Id, skill => skill);
            var addedSkills = new List<ActiveSkill>();

            foreach (var skillId in skillsId)
            {
                if (skillDictionary.TryGetValue(skillId, out var existingSkill))
                {
                    existingSkill.Add(1);
                    addedSkills.Add(existingSkill);
                }
                else
                {
                    var newCard = new ActiveSkill
                    {
                        Id = skillId,
                        Level = 1,
                        Count = 0,
                        Equipped = false,
                        EquippedSlot = -1
                    };
                    
                    ActiveSkills.Add(newCard);
                    skillDictionary[skillId] = newCard;
                    addedSkills.Add(newCard);
                }
            }
            
            
            SkillsCollected?.Invoke(addedSkills);
            SkillsCollectionChanged?.Invoke();
        }

        public void ChangeLastDropIdx(int nextIndex)
        {
            SkillsCollectedCount = nextIndex;
        }

        public void RemoveSkills()
        {
            foreach (var skill in ActiveSkills)
            {
                SkillRemoved?.Invoke(skill);
            }
            ActiveSkills?.Clear();
        }
    }
}