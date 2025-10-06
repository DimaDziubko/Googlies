using _Game.Core.Services.UserContainer;

namespace _Game.Core.UserState._Handler._Purchase
{
    public class PurchaseStateHandler : IPurchaseStateHandler
    {
        private readonly IUserContainer _userContainer;

        public PurchaseStateHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }
        
        public void AddPurchase(string id)
        {
            _userContainer.State.PurchaseDataState.AddPurchase(id);
            _userContainer.RequestSaveGame();
        }
        
        public void RemovePending(string id)
        {
            _userContainer.State.PurchaseDataState.RemovePending(id);
            _userContainer.RequestSaveGame();
        }

        public void AddPendingPurchase(string id)
        {
            _userContainer.State.PurchaseDataState.AddPending(id);
            _userContainer.RequestSaveGame();
        }
    }
}