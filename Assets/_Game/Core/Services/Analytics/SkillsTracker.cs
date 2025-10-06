using System.Collections.Generic;
using System.Linq;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._Skills.Scripts;
using DevToDev.Analytics;
using UnityEngine;

namespace _Game.Core.Services.Analytics
{
    public class SkillsTracker
    {
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private ISkillsCollectionStateReadonly SkillsState => _userContainer.State.SkillCollectionState;
        
        private readonly ISkillService _skillService;
        private readonly IMyLogger _logger;
        private readonly IUserContainer _userContainer;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IFeatureConfigRepository _config;
        private readonly IAnalyticsEventSender _sender;

        private readonly Dictionary<int, ActiveSkill> _skills = new();

        public SkillsTracker(
            IUserContainer userContainer,
            ISkillService skillService,
            IFeatureUnlockSystem featureUnlockSystem,
            IConfigRepository configRepository,
            IMyLogger logger,
            IAnalyticsEventSender sender)
        {
            _sender = sender;
            _featureUnlockSystem = featureUnlockSystem;
            _config = configRepository.FeatureConfigRepository;
            _userContainer = userContainer;
            _skillService = skillService;
            _logger = logger;
        }

        public void Initialize()
        {
            TimelineState.NextTimelineOpened += OnNextTimelineOpened;
            _skillService.SkillsAdded += OnSkillAdded;
            _featureUnlockSystem.FeatureUnlocked += OnFeatureUnlocked;
            
            foreach (var skill in _skillService.Skills.Values)
            {
                SubscribeToSkill(skill.Skill);
                _skills.Add(skill.Skill.Id, skill.Skill);
            }   
        }

        private void OnSkillAdded(List<SkillModel> skills)
        {
            foreach (var skill in skills)
            {
                if (!_skills.ContainsKey(skill.Skill.Id))
                {
                    SubscribeToSkill(skill.Skill);
                    _skills.Add(skill.Skill.Id, skill.Skill);
                }
            }

            SendPurchasedAmount(skills);
            SendPurchased(skills);
            SendUserCard();
        }

        private void SendUserCard()
        {
            int totalSkillsAmount =
                Mathf.Max(SkillsState.ActiveSkills.Sum(x => x.Count) + SkillsState.ActiveSkills.Count, 0);
            string totalSkillsAmountStr = "Total Skill Purchased";

            _sender.SetUserData(key: totalSkillsAmountStr, value: totalSkillsAmount);
            
            foreach (var activeSkill in SkillsState.ActiveSkills)
            {
                string name = $"Skill {activeSkill.Id} Level";
                _sender.SetUserData(key: name, value: activeSkill.Level);
            }
        }

        private void SendPurchasedAmount(List<SkillModel> skills)
        {
            var parameters = new DTDCustomEventParameters();
            
            parameters.Add("Quantity", skills.Count.ToString());
            parameters.Add("TimelineID", TimelineState.TimelineNumber);
            parameters.Add("Level", TimelineState.Level);
            parameters.Add("AgeID", TimelineState.AgeNumber);
            parameters.Add("BattleID", TimelineState.BattleNumber);

            _sender.CustomEvent("skill_purchased_amount", parameters);
        }

        private void SendPurchased(List<SkillModel> skills)
        {
            foreach (var skill in skills)
            {
                var parameters = new DTDCustomEventParameters();
                
                parameters.Add("Skill_ID", skill.Skill.Id);
                parameters.Add("TimelineID", TimelineState.TimelineNumber);
                parameters.Add("Level", TimelineState.Level);
                parameters.Add("AgeID", TimelineState.AgeNumber);
                parameters.Add("BattleID", TimelineState.BattleNumber);
                
                _sender.CustomEvent("skill_purchase", parameters);
            }
        }

        private void SubscribeToSkill(ActiveSkill skill)
        {
            skill.SkillEquippedChanged += OnSkillEquippedChanged;
            skill.OnAscensionLevelUp += OnSkillAscend;
        }

        private void OnSkillAscend(ActiveSkill skill)
        {
            var parameters = new DTDCustomEventParameters();
            parameters.Add("SkillId", skill.Id);
            parameters.Add("AscendTier", skill.AscensionLevel);
            parameters.Add("BattleID", TimelineState.BattleNumber);
            parameters.Add("Level", TimelineState.Level);
            parameters.Add("AgeID", TimelineState.AgeNumber);
            parameters.Add("TimelineID", TimelineState.TimelineNumber);

            _sender.CustomEvent("ascend_completed", parameters);
        }

        private void OnNextTimelineOpened()
        {
            if (TimelineState.TimelineNumber == _config.SkillRequiredTimeline && !_config.IsSkillsUnlocked)
            {
                _sender.CustomEvent("skills_not_unlocked");
            }
        }

        private void OnFeatureUnlocked(Feature feature)
        {
            if (feature == Feature.Skills)
            {
                _sender.CustomEvent("skills_unlocked");
            }
        }

        private void OnSkillEquippedChanged(ActiveSkill skill)
        {
            string name = skill.Equipped ? "skill_equipped" : "skill_unequipped";
            
            _logger.Log($"SKILL EQUIPPED EVENT {name}", DebugStatus.Success);    
            
            var parameters = new DTDCustomEventParameters();
            parameters.Add("SkillId", skill.Id);
            parameters.Add("BattleID", TimelineState.BattleNumber);
            parameters.Add("Level", TimelineState.Level);
            parameters.Add("AgeID", TimelineState.AgeNumber);
            parameters.Add("TimelineID", TimelineState.TimelineNumber);

            _sender.CustomEvent(name, parameters);
        }

        public void Dispose()
        {
            TimelineState.NextTimelineOpened -= OnNextTimelineOpened;
            _skillService.SkillsAdded -= OnSkillAdded;
            
            foreach (var skill in _skills.Values)
            {
                skill.SkillEquippedChanged -= OnSkillEquippedChanged;
                skill.OnAscensionLevelUp -= OnSkillAscend;
            }
            _skills.Clear();
            
            _featureUnlockSystem.FeatureUnlocked -= OnFeatureUnlocked;
        }
    }
}