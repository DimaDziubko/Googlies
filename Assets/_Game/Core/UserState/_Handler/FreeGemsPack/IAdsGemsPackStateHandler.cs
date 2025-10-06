using System;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.UserState._Handler.FreeGemsPack
{
    public interface IAdsGemsPackStateHandler
    {
        void RecoverAdsGemsPack(int id, int packsToAdd, DateTime newLastDailyFreePackSpent);
        void SpendAdsGemsPack(int id, DateTime lastDailyGemsPack);
    }
    
    public class AdsGemsPackStateHandler : IAdsGemsPackStateHandler
    {
        private readonly IUserContainer _userContainer;

        public AdsGemsPackStateHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }
        private void ChangeAdsGemsPack(int id, int delta, bool isPositive, DateTime lastDailyGemsPack)
        {
            delta = isPositive ? delta : (delta * -1);
            if (_userContainer.State.AdsGemsPackContainer.TryGetPack(id, out var pack))
            {
                pack.ChangeAdGemPackCount(delta, lastDailyGemsPack);
            }
            _userContainer.RequestSaveGame();
        }

        public void RecoverAdsGemsPack(int id, int packsToAdd, DateTime newLastDailyFreePackSpent)
        {
            ChangeAdsGemsPack(id, packsToAdd, true, newLastDailyFreePackSpent);
            _userContainer.RequestSaveGame();
        }

        public void SpendAdsGemsPack(int id, DateTime lastDailyGemsPack)
        {
            ChangeAdsGemsPack(id, 1, false, lastDailyGemsPack);
            _userContainer.RequestSaveGame();
        }
        
    }
}