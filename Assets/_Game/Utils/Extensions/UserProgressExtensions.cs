using _Game.Core.UserState._State;

namespace _Game.Utils.Extensions
{
    public static class UserProgressExtensions
    {
        public static bool CanPass(this UserProgress value, int timelineNumber, int ageNumber, int maxBattleNumber = 1)
        {
            if (value.TimelineNumber < timelineNumber) return true;
            if(value.TimelineNumber == timelineNumber && value.AgeNumber < ageNumber) return true;
            if(value.TimelineNumber == timelineNumber && value.AgeNumber == ageNumber && value.MaxBattleNumber <= maxBattleNumber) return true;
            return false;
        }
    }
}