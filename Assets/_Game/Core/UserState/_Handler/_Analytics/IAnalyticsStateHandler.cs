using System;

namespace _Game.Core.UserState._Handler._Analytics
{
    public interface IAnalyticsStateHandler
    {
        void AddCompletedBattle();
        void AddAdsReviewed();
        void FirstDayRetentionSent();
        void SecondDayRetentionSent();
        void TryChangeBoostDifficultCoefficientLastSentDay(DateTime currentDateDate);
        void TryChangeActivityLastSentDay(DateTime currentDate);
        void AddLevelCompleted(DateTime utcNow);
        void UpdateDate(DateTime utcNow);
    }
}