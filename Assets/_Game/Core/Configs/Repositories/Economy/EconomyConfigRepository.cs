using System;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Models._FoodBoostConfig;
using _Game.Core.Configs.Models._UpgradeItemConfig;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._UpgradesScreen.Scripts;

namespace _Game.Core.Configs.Repositories.Economy
{
    public class EconomyConfigRepository : IEconomyConfigRepository
    {
        private readonly IUserContainer _userContainer;
        private readonly ITimelineConfigRepository _timelineConfigRepository;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public EconomyConfigRepository(
            IUserContainer userContainer,
            ITimelineConfigRepository ageConfigRepository)
        {
            _userContainer = userContainer;
            _timelineConfigRepository = ageConfigRepository;
        }
        
        public FoodBoostConfig GetFoodBoostConfig() => 
            _userContainer
                .GameConfig
                .FoodBoostConfig;

        public float GetMinimalCoinsForBattle() => 
            _timelineConfigRepository
                .GetRelatedAge(TimelineState.TimelineId, TimelineState.AgeId)
                .Economy
                .CoinPerBattle;

        public UpgradeItemConfig GetConfigForType(UpgradeItemType type)
        {
            switch (type)
            {
                case UpgradeItemType.FoodProduction:
                    return _timelineConfigRepository
                        .GetRelatedAge(TimelineState.TimelineId, TimelineState.AgeId)?
                        .Economy
                        .FoodProduction;
                case UpgradeItemType.BaseHealth:
                    return _timelineConfigRepository
                        .GetRelatedAge(TimelineState.TimelineId, TimelineState.AgeId)?
                        .Economy
                        .BaseHealth;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public int GetInitialFoodAmount() =>
            _timelineConfigRepository
                .GetRelatedAge(TimelineState.TimelineId, TimelineState.AgeId)
                .Economy.InitialFoodAmount;

        public float GetInitialAmountFor(UpgradeItemType type)
        {
            switch (type)
            {
                case UpgradeItemType.FoodProduction:
                    return _timelineConfigRepository
                            .GetRelatedAge(TimelineState.TimelineId, TimelineState.AgeId)
                            .Economy.InitialFoodAmount;
                case UpgradeItemType.BaseHealth:
                    return _timelineConfigRepository
                        .GetRelatedAge(TimelineState.TimelineId, TimelineState.AgeId)
                            .Economy.BaseHealth.Value;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}