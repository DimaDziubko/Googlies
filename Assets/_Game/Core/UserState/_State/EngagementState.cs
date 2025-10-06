using System;
using System.Collections.Generic;

namespace _Game.Core.UserState._State
{
    public interface IEngagementState
    {
        float GetEngagement(int daysRange);
        public event Action ForceEngagementChanged;
    }
    public class EngagementState : IEngagementState
    {
        public event Action ForceEngagementChanged;
        public event Action EngagementChanged;
        
        //28 days here
        public List<int> LastMonthLevelCompleted;

        public DateTime LastDay;

        public float GetEngagement(int daysRange)
        {
            if (LastMonthLevelCompleted == null || LastMonthLevelCompleted.Count == 0)
                return 0;
            
            if (daysRange <= 0 || daysRange > LastMonthLevelCompleted.Count - 1)
                return 0;

            int totalLevels = 0;
            int activeDays = 0;
            
            int startIdx = LastMonthLevelCompleted.Count - 2;
            int endIdx = Math.Max(startIdx - daysRange, -1);

            for (int i = startIdx; i > endIdx; i--)
            {
                if (LastMonthLevelCompleted[i] > 0)
                {
                    totalLevels += LastMonthLevelCompleted[i];
                    activeDays++;
                }
            }

            return activeDays > 0 ? (float)totalLevels / activeDays : 0;
        }
        
        public void AddLevelCompleted(DateTime currentDate)
        {
            LastMonthLevelCompleted[^1]++;
            UpdateDate(currentDate);
        }

        public void UpdateDate(DateTime currentDate)
        {
            TryChangeDay(currentDate);
            EngagementChanged?.Invoke();
        }

        private void TryChangeDay(DateTime currentDate)
        {
            int daysPassed = (currentDate - LastDay).Days;

            if (daysPassed <= 0)
                return;

            for (int i = 0; i < daysPassed && i < LastMonthLevelCompleted.Count; i++)
            {
                LastMonthLevelCompleted.RemoveAt(0);
                LastMonthLevelCompleted.Add(0);
            }

            LastDay = currentDate;
        }

        /// <summary>
        /// Cheats
        /// </summary>
        public void ForceAddActivity()
        {
            if (LastMonthLevelCompleted.Count > 1)
            {
                LastMonthLevelCompleted[^2]++;
            }

            ForceEngagementChanged?.Invoke();
        }

        public void ForceRemoveActivity()
        {
            for (int i = LastMonthLevelCompleted.Count - 2; i >= 0; i--)
            {
                if (LastMonthLevelCompleted[i] > 0)
                {
                    LastMonthLevelCompleted[i]--;
                    ForceEngagementChanged?.Invoke();
                    return;
                }
            }
        }
    }
}