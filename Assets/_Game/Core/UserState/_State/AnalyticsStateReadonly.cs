using System;

namespace _Game.Core.UserState._State
{
    public interface IAnalyticsStateReadonly
    {
        DateTime BoostDifficultCoefficientLastSentDay { get; }
        DateTime ActivityLastSentDay { get;}
    }
    public class AnalyticsStateReadonly : IAnalyticsStateReadonly 
    {
        public DateTime BoostDifficultCoefficientLastSentDay;
        public DateTime ActivityLastSentDay;
        
        DateTime IAnalyticsStateReadonly.BoostDifficultCoefficientLastSentDay => BoostDifficultCoefficientLastSentDay;
        DateTime IAnalyticsStateReadonly.ActivityLastSentDay => ActivityLastSentDay;

        public void TryChangeBoostDifficultCoefficientLastSendDay(DateTime currentDate)
        {
            int daysPassed = (currentDate - BoostDifficultCoefficientLastSentDay).Days;

            if (daysPassed <= 0)
                return;

            BoostDifficultCoefficientLastSentDay = currentDate;
        }
        
        public void TryChangeActivityLastSentDay(DateTime currentDate)
        {
            int daysPassed = (currentDate - ActivityLastSentDay).Days;

            if (daysPassed <= 0)
                return;

            ActivityLastSentDay = currentDate;
        }
    }
}