using System;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.UserState._Handler._Analytics
{
    public class AnalyticsStateHandler : IAnalyticsStateHandler
    {
        private readonly IUserContainer _userContainer;
        public AnalyticsStateHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }
        
        public void AddAdsReviewed()
        {
            _userContainer.State.AdsStatistics.AddWatchedAd();
            _userContainer.State.AdsWeeklyWatchState.AddWatchedAd(DateTime.UtcNow);
            _userContainer.RequestSaveGame();

        }

        public void FirstDayRetentionSent()
        {
            _userContainer.State.RetentionState.ChangeFirstDayRetentionEventSent(true);
            _userContainer.RequestSaveGame();
        }

        public void SecondDayRetentionSent()
        {
            _userContainer.State.RetentionState.ChangeSecondDayRetentionEventSent(true);
            _userContainer.RequestSaveGame();

        }

        public void TryChangeBoostDifficultCoefficientLastSentDay(DateTime currentDate)
        {
            _userContainer.State.AnalyticsStateReadonly.TryChangeBoostDifficultCoefficientLastSendDay(currentDate);
            _userContainer.RequestSaveGame(true);
        }

        public void TryChangeActivityLastSentDay(DateTime currentDate)
        {
            _userContainer.State.AnalyticsStateReadonly.TryChangeActivityLastSentDay(currentDate);
            _userContainer.RequestSaveGame(true);
        }

        public void AddLevelCompleted(DateTime utcNow)
        {
            _userContainer.State.EngagementState.AddLevelCompleted(utcNow);
            _userContainer.RequestSaveGame(true);
        }

        public void UpdateDate(DateTime date)
        {
            _userContainer.State.EngagementState.UpdateDate(date);
            _userContainer.RequestSaveGame(true);
        }

        public void AddCompletedBattle()
        {
            _userContainer.State.BattleStatistics.AddCompletedBattle();
            _userContainer.RequestSaveGame();
        }
    }
}