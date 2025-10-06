using _Game.Core.UserState._State;
using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP
{
    public interface IPurchaseProcessListener
    {
        PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs eventArgs);
        void ProcessPendingPurchase(PendingIAP pendingIaP);
    }
}