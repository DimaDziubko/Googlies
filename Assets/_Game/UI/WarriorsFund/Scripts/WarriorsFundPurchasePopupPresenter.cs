using System;
using _Game.Core._Logger;
using _Game.Core.Services.IAP;
using _Game.LiveopsCore.Models.WarriorsFund;
using UnityEngine.Purchasing;

namespace _Game.UI.WarriorsFund.Scripts
{
    public class WarriorsFundPurchasePopupPresenter
    {
        public event Action Purchased;
        
        private readonly WarriorsFundEvent _event;
        private readonly IAPProvider _iapProvider;
        private readonly IMyLogger _logger;

        public WarriorsFundPurchasePopupPresenter(
            WarriorsFundEvent @event,
            IAPProvider iAPProvider,
            IMyLogger logger)
        {
            _logger = logger;
            _event = @event;
            _iapProvider = iAPProvider;
        }

        public void Initialize() => _event.Purchased += OnPurchased;
        public void Dispose() => _event.Purchased -= OnPurchased;

        private void OnPurchased()
        {
            _logger.Log("WF PURCHASE POPUP PRESENTER ON PURCHASED", DebugStatus.Info);
            Purchased?.Invoke();
        }
        
        public Product GetSegmentedIAP()
        {
            var segmentedProduct = _event.SegmentIAP;
            if (segmentedProduct == null)
                return null;
            
            _iapProvider.AllProducts.TryGetValue(segmentedProduct.ProductId, out var product);
            
            return product;
        }

        public void OnPurchaseButtonClicked() => 
            _iapProvider.StartPurchase(GetSegmentedIAP().definition.id);
    }
}