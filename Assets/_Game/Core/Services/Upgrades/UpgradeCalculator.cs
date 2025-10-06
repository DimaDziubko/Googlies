using System;
using _Game.Core._DataPresenters._UpgradeItem;
using _Game.Core._GameInitializer;
using _Game.Core.Configs.Models._Functions;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Economy;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._UpgradesScreen.Scripts;
using UnityEngine;

namespace _Game.Core.Services.Upgrades
{
    public class UpgradeCalculator : IUpgradeCalculator, IDisposable
    {
        private readonly IEconomyConfigRepository _economyConfigRepository;
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IAgeNavigator _ageNavigator;
        private readonly UpgradeItemContainer _upgradeItemContainer;
        private readonly ITimelineNavigator _timelineNavigator;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public  UpgradeCalculator(
            IConfigRepository configRepository, 
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IAgeNavigator ageNavigator,
            ITimelineNavigator timelineNavigator,
            UpgradeItemContainer upgradeItemContainer)
        {
            _economyConfigRepository = configRepository.EconomyConfigRepository;
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _ageNavigator = ageNavigator;
            _upgradeItemContainer = upgradeItemContainer;
            _timelineNavigator = timelineNavigator;
            gameInitializer.OnMainInitialization += Init;
        }

        private void Init()
        {
            TimelineState.UpgradeItemLevelChanged += OnUpgradeItemLevelChanged;
            _ageNavigator.AgeChanged += OnAgeChanged;
            _timelineNavigator.TimelineChanged += OnTimelineChanged;
            UpdateUpgradeValues();
        }

        void IDisposable.Dispose()
        {
            TimelineState.UpgradeItemLevelChanged -= OnUpgradeItemLevelChanged;
            _ageNavigator.AgeChanged -= OnAgeChanged;
            _timelineNavigator.TimelineChanged -= OnTimelineChanged;
            _gameInitializer.OnMainInitialization -= Init;
        }

        private void OnTimelineChanged() => UpdateUpgradeValues();

        private void OnAgeChanged() => UpdateUpgradeValues();

        private void UpdateUpgradeValues()
        {
            foreach (var item in _upgradeItemContainer.GetAllItems())
            {
                UpdateUpgradeValue(item, TimelineState.GetUpgradeLevel(item.Type));
            }
        }
        
        private void OnUpgradeItemLevelChanged(UpgradeItemType type, int level)
        {
            var model = _upgradeItemContainer.GetItem(type);
            UpdateUpgradeValue(model, level);
        }

        private void UpdateUpgradeValue(UpgradeItemModel model, int level)
        {
            float initialAmount = _economyConfigRepository.GetInitialAmountFor(model.Type);
            float amount = CalculateAmountForType(model.Type, level);
            float price = CalculatePriceForType(model.Type, level);
            
            UpgradeItemDynamicData newValue = new UpgradeItemDynamicData
            {
                Value = amount,
                Price = price,
                InitialValue = initialAmount
            };
            
            model.SetData(newValue);
        }

        private float CalculatePriceForType(UpgradeItemType type, int level)
        {
            var config = _economyConfigRepository.GetConfigForType(type);
            return CalculatePrice(config.Price, config.PriceExponential, level);
        }
        private float CalculateAmountForType(UpgradeItemType type, int level)
        {
            var config = _economyConfigRepository.GetConfigForType(type);
            return config.Value + config.ValueStep * level;
        }

        private float CalculatePrice(float basePrice, Exponential growth, int level) => 
            Mathf.Round(basePrice + growth.GetValue(level));
    }
}