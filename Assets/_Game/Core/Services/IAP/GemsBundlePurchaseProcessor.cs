using _Game.Core.Boosts;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._RewardProcessing;
using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP
{
    public class GemsBundlePurchaseProcessor : IPurchaseProcessListener
    {
        private readonly GemsBundleContainer _container;
        private readonly RewardProcessingService _rewardProcessingService;
        private readonly IUserContainer _userContainer;

        public GemsBundlePurchaseProcessor(
            GemsBundleContainer container,
            RewardProcessingService rewardProcessingService,
            IUserContainer userContainer)
        {
            _userContainer = userContainer;
            _container = container;
            _rewardProcessingService = rewardProcessingService;
        }
        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs eventArgs)
        {
            string id = eventArgs.purchasedProduct.definition.id;
            
            if (_container.TryGetValue(id, out GemsBundle bundle))
            {
                if (bundle == null) return PurchaseProcessingResult.Complete;
                HandleGemsBundlePurchase(eventArgs, bundle);
            }
            
            return PurchaseProcessingResult.Complete;
        }

        public void ProcessPendingPurchase(PendingIAP pendingIaP)
        {
            if (_container.TryGetValue(pendingIaP.IAPId, out GemsBundle bundle))
            {
                ValidateAndGrantReward(bundle);
                _userContainer.PurchaseStateHandler.RemovePending(pendingIaP.IAPId);
            }
        }

        private void HandleGemsBundlePurchase(PurchaseEventArgs eventArgs, GemsBundle bundle)
        {
            if (ValidateAndGrantReward(bundle)) return;
        }

        private bool ValidateAndGrantReward(GemsBundle bundle)
        {
            if (bundle == null) return true;
            _rewardProcessingService.Process(bundle.Reward, ItemSource.Shop);
            _userContainer.PurchaseStateHandler.AddPurchase(bundle.Id);
            return false;
        }
    }
}