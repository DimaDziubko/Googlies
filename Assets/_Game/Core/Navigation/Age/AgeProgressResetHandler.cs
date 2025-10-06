using System;
using _Game.Core._Logger;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using Zenject;

namespace _Game.Core.Navigation.Age
{
    public class AgeProgressResetHandler : 
        IInitializable, 
        IDisposable,
        ISaveGameTrigger
    {
        public event Action<bool> SaveGameRequested;
        
        private readonly IUserContainer _userContainer;
        private readonly IAgeNavigator _ageNavigator;
        private readonly IMyLogger _logger;
        private readonly ITimelineNavigator _timelineNavigator;
        private readonly CurrencyBank _bank;

        private CurrencyCell CoinsCell => _bank.GetCell(CurrencyType.Coins);

        public AgeProgressResetHandler(
            CurrencyBank bank,
            IAgeNavigator ageNavigator,
            ITimelineNavigator timelineNavigator,
            IMyLogger logger)
        {
            _bank = bank;
            _ageNavigator = ageNavigator;
            _timelineNavigator = timelineNavigator;
            _logger = logger;
        }

        void IInitializable.Initialize()
        {
            _ageNavigator.AgeChanged += OnAgeChanged;
            _timelineNavigator.TimelineChanged += OnAgeChanged;
        }

        private void OnAgeChanged()
        {
            CoinsCell.Change(0);
            SaveGameRequested?.Invoke(true);
            _logger.Log("RESET COINS", DebugStatus.Warning);
        }

        void IDisposable.Dispose()
        {
            _ageNavigator.AgeChanged -= OnAgeChanged;
            _timelineNavigator.TimelineChanged -= OnAgeChanged;
        }
    }
}