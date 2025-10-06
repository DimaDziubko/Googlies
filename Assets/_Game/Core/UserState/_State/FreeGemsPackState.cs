using System;

namespace _Game.Core.UserState._State
{
    public class FreeGemsPackState : IFreeGemsPackStateReadonly
    {
        public int Id;
        public int FreeGemPackCount;
        public DateTime LastFreeGemPackDay;

        public event Action<int, int> FreeGemsPackCountChanged;

        int IFreeGemsPackStateReadonly.FreeGemPackCount => FreeGemPackCount;
        DateTime IFreeGemsPackStateReadonly.LastFreeGemPackDay => LastFreeGemPackDay;

        public void ChangeAdGemPackCount (int delta, DateTime lastDailyFoodBoost)
        {
            FreeGemPackCount += delta;
            LastFreeGemPackDay = lastDailyFoodBoost;
            
            FreeGemsPackCountChanged?.Invoke(Id, FreeGemPackCount);
        }
    }
}