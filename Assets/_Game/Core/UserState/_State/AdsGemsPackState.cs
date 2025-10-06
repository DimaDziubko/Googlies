using System;

namespace _Game.Core.UserState._State
{
    public class AdsGemsPackState : IAdsGemsPackStateReadonly
    {
        public int Id;
        public int AdsGemPackCount;
        public DateTime LastAdsGemPackDay;

        public event Action<int, int> AdsGemsPackCountChanged;

        int IAdsGemsPackStateReadonly.AdsGemPackCount => AdsGemPackCount;
        DateTime  IAdsGemsPackStateReadonly.LastAdsGemPackDay => LastAdsGemPackDay;

        public void ChangeAdGemPackCount (int delta, DateTime lastDailyFoodBoost)
        {
            AdsGemPackCount += delta;
            LastAdsGemPackDay = lastDailyFoodBoost;
            
            AdsGemsPackCountChanged?.Invoke(Id, AdsGemPackCount);
        }
    }
}