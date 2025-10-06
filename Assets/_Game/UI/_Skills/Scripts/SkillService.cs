using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._Skills;
using _Game.Core.Configs.Repositories;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.Random;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI.Common.Scripts;
using Sirenix.OdinInspector;

namespace _Game.UI._Skills.Scripts
{
    public class SkillService : 
        ISkillService, 
        IDisposable,
        ISaveGameTrigger
    {
        public event Action<bool> SaveGameRequested;
        public event Action<List<SkillModel>> SkillsAdded;
        public bool IsAnyUpgradeAvailable => SkillsState.ActiveSkills.Any(x => x.Count >= _config.ForSkill(x.Id).GetUpgradeCount(x.Level));
        public string SkillProgressInfo => $"Skills\n<style=Smaller>{SkillsState.ActiveSkills.Count}/{_config.GetAllSkillsCount()}</style>";


        [ShowInInspector]
        public IReadOnlyDictionary<SkillType, SkillModel> Skills => _skills;
        
        public SkillSlotContainer SkillSlotContainer => _slotContainer;

        private Dictionary<SkillType, SkillModel> _skills;
        
        [ShowInInspector]
        private readonly SkillSlotContainer _slotContainer;

        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly ISkillConfigRepository _config;
        private readonly ITimelineNavigator _timelineNavigator;
        private readonly IMyLogger _logger;
        private readonly IRandomService _random;

        private SkillGenerator _generator;

        public SkillGenerator Generator => _generator;

        private ISkillsCollectionStateReadonly SkillsState => _userContainer.State.SkillCollectionState;
        
        public SkillService(
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IConfigRepository configRepository,
            ITimelineNavigator timelineNavigator,
            IRandomService random,
            IMyLogger logger)
        {
            _random = random;
            _logger = logger;
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _config = configRepository.SkillConfigRepository;
            _timelineNavigator = timelineNavigator;
            _slotContainer = new SkillSlotContainer(logger);

            _gameInitializer.OnMainInitialization += Init;
        }

        private void Init()
        {
            _generator = new SkillGenerator(_config, _random, _logger, SkillsState);
            
            Subscribe();   
            
            InitSlots();
            InitSkills();
        }

        public float GetX1SkillPrice() => _config.GetX1SkillPrice();
        public float GetX10SkillPrice() => _config.GetX10SkillPrice();

        public void SkillBundleBought(IProduct product)
        {
            if (product is SkillBundle skillBundle)
            {
                List<int> skillIds = _generator.GenerateSkills(skillBundle.Quantity);
                _userContainer.UpgradeStateHandler.AddSkills(skillIds);
            }
            else
            {
                _logger.Log($"Wrong type {product.Title}", DebugStatus.Warning);
            }
        }
        
        public SkillModel GetSkillModel(SkillType type) => 
            Skills.GetValueOrDefault(type);

        private void Subscribe()
        {
            _timelineNavigator.TimelineChanged += OnTimelineChanged;
            SkillsState.SkillsCollected += OnSkillsCollected;
            SkillsState.SkillRemoved += OnSkillRemoved;
        }

        private void OnSkillRemoved(ActiveSkill skill)
        {
            SkillModel skillToRemove = _skills.FirstOrDefault(x => x.Value.Skill.Id == skill.Id).Value;
            if (skillToRemove != null)
            {
                foreach (var slot in _slotContainer.GetSlots())
                {
                    if (slot.SkillModel == skillToRemove)
                    {
                        slot.UnEquip(skillToRemove);
                    }
                }
                _skills.Remove(skillToRemove.Type);
            }
        }

        private void UnSubscribe()
        {
            SkillsState.SkillsCollected -= OnSkillsCollected;
            _timelineNavigator.TimelineChanged -= OnTimelineChanged;
            _gameInitializer.OnMainInitialization -= Init;
            SkillsState.SkillRemoved -= OnSkillRemoved;
        }

        private void OnSkillsCollected(List<ActiveSkill> newSkills)
        {
            List<SkillModel> addedSkills = new List<SkillModel>();
            
            foreach (var skill in newSkills)
            {
                var model = new SkillModel(_config.ForSkill(skill.Id), skill);
                _skills[model.Type] = model;
                addedSkills.Add(model);
            }
            
            SkillsAdded?.Invoke(addedSkills);
            SaveGameRequested?.Invoke(false);
        }

        private void OnTimelineChanged()
        {
            CheckSlotsLocked();
        }

        private void CheckSlotsLocked()
        {
            var slots = _slotContainer.GetSlots();

            foreach (var slot in slots)
            {
                slot.SetLocked(_config.TimelineThresholdForSlot(slot.Id) > _timelineNavigator.CurrentTimelineNumber);
                slot.SetTimelineThreshold(_config.TimelineThresholdForSlot(slot.Id));
            }
        }

        private void InitSlots() => CheckSlotsLocked();

        private void InitSkills()
        {
            var sortedSkills = SkillsState.ActiveSkills
                .OrderByDescending(skill => skill.Equipped) 
                .ThenBy(skill => skill.Equipped ? skill.EquippedSlot : int.MaxValue)
                .ToList();
            
            _skills = new Dictionary<SkillType, SkillModel>();
            
            foreach (var skill in sortedSkills)
            {
                var model = new SkillModel(_config.ForSkill(skill.Id), skill);
                _skills[model.Type] = model;

                if (skill.Equipped)
                {
                    _slotContainer.Equip(model);
                }
            }
        }

        void IDisposable.Dispose()
        {
            UnSubscribe();
        }
    }
}