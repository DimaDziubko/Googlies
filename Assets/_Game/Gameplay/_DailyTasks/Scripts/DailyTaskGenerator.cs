using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Boosts;
using _Game.Core.Configs.Models._DailyTaskConfig;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.DailyTask;
using _Game.Core.Services.Random;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._UpgradesScreen.Scripts;

namespace _Game.Gameplay._DailyTasks.Scripts
{
    public class DailyTaskGenerator
    {
        private readonly IDailyTaskConfigRepository _configRepository;
        private readonly IUserContainer _userContainer;
        private readonly IRandomService _random;
        
        private IDailyTasksStateReadonly State => _userContainer.State.DailyTasksState;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public DailyTaskGenerator(
            IConfigRepository configRepository,
            IUserContainer userContainer,
            IRandomService random)
        {
            _configRepository = configRepository.DailyTaskConfigRepository;
            _userContainer = userContainer;
            _random = random;
        }
        
        public DailyTaskModel TryGetPendingDailyTask()
        {
            if (State.CurrentTaskIdx < 0)
                return null;

            var allConfigs = _configRepository.GetDailyTaskConfigs();
            var config = allConfigs.Find(c => c.Id == State.CurrentTaskIdx);
            if (config == null)
            {
                throw new InvalidOperationException($"Config for ID={State.CurrentTaskIdx} not found!");
            }
            
            DailyTasksState state = _userContainer.State.DailyTasksState;
            return CreateDailyTaskModel(config, state);
        }
        
        public DailyTaskModel GenerateNewRandomTask()
        {
            var allConfigs = _configRepository.GetDailyTaskConfigs();
            
            var config = SelectRandomConfig(allConfigs);
            
            DailyTasksState state = _userContainer.State.DailyTasksState;
            state.ClearProgress();
            return CreateDailyTaskModel(config, state);
        }

        private DailyTaskModel CreateDailyTaskModel(DailyTaskConfig config, DailyTasksState state)
        {
            state.ChangeCurrentTaskIdx(config.Id);
            int upgradeLevel = TimelineState.GetUpgradeLevel(UpgradeItemType.FoodProduction);
            
            int target = config.LinearFunctions
                             .FirstOrDefault()
                             ?.GetInt(upgradeLevel) 
                         ?? 0;
            
            var model = new DailyTaskModel.DailyTaskModelBuilder()
                .WithState(state)
                .WithType(config.DailyTaskType)
                .WithDescription(config.Description)
                .WithTarget(target)
                .WithTargetFunctions(config.LinearFunctions)
                .WithReward(new []{new CurrencyData() {Type = CurrencyType.Gems, Amount = config.Reward, Source = ItemSource.DailyTask}})
                .WithDailyInfo($"Daily {state.CompletedTasks.Count + 1}/{_configRepository.MaxDailyCountPerDay}")
                .Build();
            
            return model;
        }
        
        private DailyTaskConfig SelectRandomConfig(List<DailyTaskConfig> configs)
        {
            int totalDropChance = 0;
            foreach (var c in configs)
            {
                if (!State.CompletedTasks.Contains(c.Id))
                    totalDropChance += c.DropChance;
            }

            if (totalDropChance == 0)
                throw new InvalidOperationException("No valid tasks available for selection.");

            int randomPoint = _random.Next(0, totalDropChance);
            int cumulativeChance = 0;
            foreach (var c in configs)
            {
                if (State.CompletedTasks.Contains(c.Id))
                    continue;

                cumulativeChance += c.DropChance;
                
                if (randomPoint < cumulativeChance)
                {
                    return c;
                }
            }

            throw new InvalidOperationException("Failed to select a daily task by drop chance.");
        }
    }
}