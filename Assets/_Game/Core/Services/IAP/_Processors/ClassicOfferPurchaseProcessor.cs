using System.Collections;
using _Game.Core._GameSaver;
using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._RewardProcessing;
using _Game.Gameplay.Common;
using _Game.LiveopsCore.Models.ClassicOffer;
using UnityEngine;
using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP._Processors
{
    public class ClassicOfferPurchaseProcessor : IPurchaseProcessListener
    {
        private readonly BalancyPaymentTracker _tracker;
        private readonly ClassicOfferEvent _event;
        private readonly ICoroutineRunner _runner;
        private readonly RewardProcessingService _rewardProcessing;
        private readonly PurchaseChecker _checker;
        private readonly IIAPService _iapService;
        private readonly IUserContainer _userContainer;
        private readonly SaveGameMediator _saveMediator;
        private readonly IMyLogger _logger;
        private Coroutine _claimCoroutine;


        public ClassicOfferPurchaseProcessor(
            PurchaseChecker checker,
            ClassicOfferEvent @event,
            BalancyPaymentTracker validatorTracker,
            RewardProcessingService rewardProcessingService,
            IUserContainer userContainer,
            ICoroutineRunner coroutine,
            SaveGameMediator saveMediator,
            IIAPService iIAPService,
            IMyLogger logger
            )
        {
            _checker = checker;
            _userContainer = userContainer;
            _event = @event;
            _tracker = validatorTracker;
            _runner = coroutine;
            _rewardProcessing = rewardProcessingService;
            _iapService = iIAPService;
            _saveMediator = saveMediator;
            _logger = logger;
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
                    ClaimAllItems();

                    return PurchaseProcessingResult.Complete;
                }
            }

            return PurchaseProcessingResult.Complete;
        }

        public void ProcessPendingPurchase(PendingIAP pendingIaP)
        {
            foreach (var price in _event.IAP.AllPrices)
            {
                if (pendingIaP.IAPId == price.Product.ProductId)
                {
                    _event.PurchaseData.AddPurchase(pendingIaP.IAPId);
                    ChangePurchased();
                    _saveMediator.SaveGame(false);
                    ClaimAllItems();
                }
            }
        }

        private void ClaimAllItems()
        {
            if (_claimCoroutine != null)
                _runner.StopCoroutine(_claimCoroutine);

            _claimCoroutine = _runner.StartCoroutine(ClaimAllItemsCoroutine());
            _event.PurchaseOffer();
        }

        private IEnumerator ClaimAllItemsCoroutine()
        {
            foreach (var item in _event.Rewards)
            {
                _rewardProcessing.Process(item, ItemSource.ClassicOffer);
                yield return new WaitForSeconds(0.5f);
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
