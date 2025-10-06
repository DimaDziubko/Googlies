using System;
using _Game.Core._GameInitializer;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState;

namespace _Game.Core._RetentionChecker
{
    public class RetentionChecker : IRetentionChecker, IDisposable
    {
        private const int FIRST_DAY_RETENTION_MIN_HOURS = 24;
        private const int FIRST_DAY_RETENTION_MAX_HOURS = 48;
        
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;

        private IRetentionStateReadonly RetentionState => _userContainer.State.RetentionState;

        public RetentionChecker(
            IUserContainer userContainer,
            IGameInitializer gameInitializer)
        {
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init() => 
            CheckRetention();

        private void CheckRetention()
        {
            CheckFirstDayRetention();
            CheckSecondDayRetention();
        }

        private void CheckSecondDayRetention()
        {
            if (RetentionState.FirstDayRetentionEventSent 
                && !RetentionState.SecondDayRetentionEventSent)
                _userContainer.AnalyticsStateHandler.SecondDayRetentionSent();
        }

        private void CheckFirstDayRetention()
        {
            if (!RetentionState.FirstDayRetentionEventSent)
            {
                var deltaTime = DateTime.UtcNow - RetentionState.FirstOpenTime;
                if (deltaTime.TotalHours >= FIRST_DAY_RETENTION_MIN_HOURS && 
                    deltaTime.TotalHours < FIRST_DAY_RETENTION_MAX_HOURS)
                {
                    _userContainer.AnalyticsStateHandler.FirstDayRetentionSent();
                }
            }
        }

        void IDisposable.Dispose() => 
            _gameInitializer.OnPostInitialization -= Init;
    }

    public interface IRetentionChecker
    {
    }
}