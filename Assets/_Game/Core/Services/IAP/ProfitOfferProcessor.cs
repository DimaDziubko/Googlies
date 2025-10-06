using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._RewardProcessing;
using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP
{
    public class ProfitOfferProcessor : 
        IPurchaseProcessListener, 
        IRestoreProcessListener
    {
        private readonly ProfitOfferContainer _container;
        private readonly IUserContainer _userContainer;
        private readonly IMyLogger _logger;
        private readonly RewardProcessingService _rewardProcessingService;
        private readonly PurchaseChecker _purchaseChecker;
        
        public ProfitOfferProcessor(
            ProfitOfferContainer  container,
            IUserContainer userContainer,
            RewardProcessingService rewardProcessingService,
            IMyLogger logger,
            PurchaseChecker purchaseChecker)
        {
            _purchaseChecker = purchaseChecker;
            _logger = logger;
            _userContainer = userContainer;
            _container = container;
            _rewardProcessingService = rewardProcessingService;
        }

        PurchaseProcessingResult IPurchaseProcessListener.ProcessPurchase(PurchaseEventArgs eventArgs)
        {
            string id = eventArgs.purchasedProduct.definition.id;
            
            if (_container.TryGetValue(id, out ProfitOfferModel offer))
            {
                if (offer == null) return PurchaseProcessingResult.Complete;
                HandleProfitOfferPurchase(eventArgs, offer);
            }
            
            _userContainer.PurchaseStateHandler.RemovePending(id);
            return PurchaseProcessingResult.Complete;
        }

        void IPurchaseProcessListener.ProcessPendingPurchase(PendingIAP pendingIaP)
        {
            if (_container.TryGetValue(pendingIaP.IAPId, out ProfitOfferModel offer))
            {
                if (offer == null) return;
                
                ValidateAndGrantOffer(offer);
                _userContainer.PurchaseStateHandler.RemovePending(pendingIaP.IAPId);
            }
        }

        private void HandleProfitOfferPurchase(PurchaseEventArgs eventArgs, ProfitOfferModel offer)
        {
            if (offer == null) return;
            
            if (offer.IsActive)
            {
                _logger.Log("Profit offer already purchased.");
                return;
            }

            GrantProfitOfferReward(offer);
        }


        void IRestoreProcessListener.OnRestore()
        {
            foreach (var offer in _container.GetAll()) 
                ValidateAndGrantOffer(offer);
        }

        private void ValidateAndGrantOffer(ProfitOfferModel offer)
        {
            if (offer.HasReceipt() && !_purchaseChecker.IsBought(offer.Id))
            {
                GrantProfitOfferReward(offer);
            }
        }

        private void GrantProfitOfferReward(ProfitOfferModel offer)
        {
            foreach (var reward in offer.Rewards) 
                _rewardProcessingService.Process(reward, ItemSource.Shop);

            offer.SetActive(true);
            _userContainer.PurchaseStateHandler.AddPurchase(offer.Id);
        }
    }
}