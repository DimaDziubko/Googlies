namespace _Game.Core.UserState._Handler._Purchase
{
    public interface IPurchaseStateHandler
    {
        void AddPurchase(string id);
        void RemovePending(string id);
        void AddPendingPurchase(string id);
    }
}