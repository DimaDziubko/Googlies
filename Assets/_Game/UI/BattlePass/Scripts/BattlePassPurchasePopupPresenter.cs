using System;
using _Game.Core._Logger;
using _Game.Core.Services.IAP;
using _Game.LiveopsCore.Models.BattlePass;
using UnityEngine.Purchasing;

namespace _Game.UI.BattlePass.Scripts
{
    public class BattlePassPurchasePopupPresenter
    {
        public event Action<float> EventTimerTick;
        public event Action Purchased;
        
        private readonly BattlePassEvent _event;
        private readonly IAPProvider _iapProvider;
        private readonly IMyLogger _logger;

        public BattlePassPurchasePopupPresenter(
            BattlePassEvent @event,
            IAPProvider iAPProvider,
            IMyLogger logger)
        {
            _logger = logger;
            _event = @event;
            _iapProvider = iAPProvider;
        }

        public void Initialize()
        {
            _event.EventTimerTick += OnEventTimerTick;
            _event.Purchased += OnPurchased;
        }

        public void Dispose()
        {
            _event.EventTimerTick -= OnEventTimerTick;
            _event.Purchased -= OnPurchased;
        }

        private void OnPurchased()
        {
            _logger.Log("PURCHASE POPUP PRESENTER ON PURCHASED", DebugStatus.Info);
            Purchased?.Invoke();
        }
        
        private void OnEventTimerTick(float remainingSeconds)
        {
            EventTimerTick?.Invoke(remainingSeconds);
        }

        public Product GetSegmentedIAP()
        {
            var segmentedProduct = _event.SegmentIAP;
            if (segmentedProduct == null)
            {
                return null;
            }
            
            _iapProvider.AllProducts.TryGetValue(segmentedProduct.ProductId, out var product);
            
            return product;
        }

        public void OnPurchaseButtonClicked() => 
            _iapProvider.StartPurchase(GetSegmentedIAP().definition.id);
    }
}