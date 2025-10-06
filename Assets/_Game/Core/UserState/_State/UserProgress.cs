namespace _Game.Core.UserState._State
{
    public struct UserProgress
    {
        public int TimelineNumber;
        public int AgeNumber;
        public int MaxBattleNumber;

        public int GetAgeSerialNumber(int agesInTimeline) => 
            (TimelineNumber - 1) * agesInTimeline + AgeNumber;
    }
}