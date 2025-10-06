using System;
using System.Collections.Generic;
using System.Globalization;
using _Game.Core.Boosts;
using _Game.Core.Configs.Repositories;
using _Game.Core.Data;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using _Game.Utils;
using DevToDev.Analytics;
using Zenject;

namespace _Game.Gameplay._Boosts.Scripts
{
    public class BoostsTracker : 
        IInitializable,
        IDisposable, 
        IGameScreenListener<IMenuScreen>
    {
        private readonly BoostContainer _container;
        private readonly IAnalyticsEventSender _sender;

        private readonly Dictionary<BoostType, float> _cachedValues = new();
        
        private readonly IUserContainer _userContainer;
        private readonly IConfigRepository _config;

        private IAnalyticsStateReadonly Analytics => _userContainer.State.AnalyticsStateReadonly;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        
        public BoostsTracker(
            IAnalyticsEventSender sender,
            BoostContainer container,
            IUserContainer userContainer,
            IConfigRepository config)
        {
            _container = container;
            _sender = sender;
            _userContainer = userContainer;
            _config = config;
        }


        void IInitializable.Initialize()
        {
            foreach (var model in _container.GetBoostModels(BoostSource.Total))
            {
                model.DetailedChanged += OnModelChanged;
            }
            
        }

        void IDisposable.Dispose()
        {
            foreach (var model in _container.GetBoostModels(BoostSource.Total))
            {
                model.DetailedChanged -= OnModelChanged;
            }
        }

        private void OnModelChanged(BoostType type, float newValue)
        {
            if (!_cachedValues.ContainsKey(type) || Math.Abs(_cachedValues[type] - newValue) > Constants.ComparisonThreshold.EPSILON)
            {
                _cachedValues[type] = newValue;
                
                _sender.SetUserData(type.ToString(), newValue.ToString(CultureInfo.InvariantCulture));
            }
        }

        void IGameScreenListener<IMenuScreen>.OnScreenOpened(IMenuScreen screen)
        {
            TrySendBoostDifficultyCoefficient();
        }

        private void TrySendBoostDifficultyCoefficient()
        {
            DateTime currentDate = DateTime.UtcNow;
            
            int daysPassed = (currentDate - Analytics.BoostDifficultCoefficientLastSentDay).Days;

            if (daysPassed <= 0)
                return;

            float attack = _container.GetBoostValue(BoostSource.Total, BoostType.AllUnitDamage);
            float hp = _container.GetBoostValue(BoostSource.Total, BoostType.AllUnitHealth);
            float power = (attack + hp) / 2;
            float difficulty = _config.DifficultyConfigRepository.GetDifficultyValue(TimelineState.TimelineNumber);
            
            var attackCoef = Math.Round(attack / difficulty, 3);
            var hpCoef = Math.Round(hp / difficulty, 3);
            var powerCoef = Math.Round(power / difficulty, 3);
            
            DTDCustomEventParameters parameters = new DTDCustomEventParameters();
            
            parameters.Add("attack_coef", attackCoef);
            parameters.Add("hp_coef", hpCoef);
            parameters.Add("power_coef", powerCoef);
            
            _sender.CustomEvent("power_params", parameters);
            _userContainer.AnalyticsStateHandler.TryChangeBoostDifficultCoefficientLastSentDay(currentDate.Date);
        }

        void IGameScreenListener<IMenuScreen>.OnInfoChanged(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnRequiresAttention(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnScreenClosed(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnScreenActiveChanged(IMenuScreen screen, bool isActive) { }
        void IGameScreenListener<IMenuScreen>.OnScreenDisposed(IMenuScreen screen) { }
    }
}