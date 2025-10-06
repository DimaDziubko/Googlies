using System;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.UserState._Handler.FreeGemsPack
{
    public class FreeGemsPackStateHandler : IFreeGemsPackStateHandler
    {
        private readonly IUserContainer _userContainer;

        public FreeGemsPackStateHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }
        private void ChangeFreeGemsPack(int id, int delta, bool isPositive, DateTime lastDailyGemsPack)
        {
            delta = isPositive ? delta : (delta * -1);
            if (_userContainer.State.FreeGemsPackContainer.TryGetPack(id, out var pack))
            {
                pack.ChangeAdGemPackCount(delta, lastDailyGemsPack);
            }
            _userContainer.RequestSaveGame();
        }

        public void RecoverFreeGemsPack(int id, int packsToAdd, DateTime newLastDailyFreePackSpent)
        {
            ChangeFreeGemsPack(id, packsToAdd, true, newLastDailyFreePackSpent);
            _userContainer.RequestSaveGame();
        }

        public void SpendFreeGemsPack(int id, DateTime lastDailyGemsPack)
        {
            ChangeFreeGemsPack(id, 1, false, lastDailyGemsPack);
            _userContainer.RequestSaveGame();
        }
    }
}