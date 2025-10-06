using System;

namespace _Game.Core.UserState._Handler.FreeGemsPack
{
    public interface IFreeGemsPackStateHandler
    {
        void RecoverFreeGemsPack(int id, int packsToAdd, DateTime newLastDailyFreePackSpent);
        void SpendFreeGemsPack(int id, DateTime lastDailyGemsPack);
    }
}