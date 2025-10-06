using _Game.Core._GameSaver;
using _Game.Core._Logger;
using _Game.Core.Services.IAP;
using _Game.Core.UserState._State;
using _Game.LiveopsCore.Models.BattlePass;
using _Game.LiveopsCore.Models.WarriorsFund;
using UnityEngine.Purchasing;

namespace _Game.LiveopsCore._GameEventStrategies
{
    public class WarriorsFundPurchaseProcessor : IPurchaseProcessListener
    {
        private readonly BalancyPaymentTracker _tracker;
        private readonly WarriorsFundEvent _event;
        private readonly PurchaseChecker _checker;
        private readonly IIAPService _iapService;
        private readonly IMyLogger _logger;
        private readonly SaveGameMediator _saveMediator;

        public WarriorsFundPurchaseProcessor(
            PurchaseChecker checker,
            WarriorsFundEvent @event,
            BalancyPaymentTracker tracker,
            SaveGameMediator saveMediator,
            IIAPService iIAPService,
            IMyLogger logger)
        {
            _saveMediator = saveMediator;
            _logger = logger;
            _checker = checker;
            _tracker = tracker;
            _event = @event;
            _iapService = iIAPService;
        }

        public void Initialize()
        {
            _iapService.RegisterPurchaseListener(this);
            SetPurchased();
        }

        public void Dispose() => 
            _iapService.UnregisterPurchaseListener(this);

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs eventArgs)
        {
            foreach (var price in _event.IAP.AllPrices)
            {
                if (eventArgs.purchasedProduct.definition.id == price.Product.ProductId)
                {
                    _event.PurchaseData.AddPurchase(eventArgs.purchasedProduct.definition.id);
                    ChangePurchased();
                    _tracker.SendItemPurchased(_event.IAP, eventArgs);
                    _saveMediator.SaveGame(false);
                    return PurchaseProcessingResult.Complete;
                }
            }
            
            return PurchaseProcessingResult.Complete;
        }

        public void ProcessPendingPurchase(PendingIAP pendingIaP)
        {
            foreach (var price in  _event.IAP.AllPrices)
            {
                if (pendingIaP.IAPId == price.Product.ProductId)
                {
                    _event.PurchaseData.AddPurchase(pendingIaP.IAPId);
                    ChangePurchased();
                    _saveMediator.SaveGame(false);
                }
            }
        }

        private void SetPurchased()
        {
            _event.SetPurchased(IsPurchased());
            _logger.Log($"SET PURCHASED {IsPurchased()}", DebugStatus.Info);
        }

        private void ChangePurchased()
        {
            _logger.Log($"CHANGE PURCHASED BP PROCESSOR IS PURCHASED:{IsPurchased()}", DebugStatus.Info);
            _event.ChangePurchased(IsPurchased());
        }

        private bool IsPurchased() => 
            _checker.IsBought(_event.PurchaseData, _event.IAP.AllPrices);
    }
}