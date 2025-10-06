using _Game.Core._Logger;
using Balancy;
using Balancy.API.Payments;
using Balancy.Models.SmartObjects;
using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP
{
    public class BalancyPaymentTracker
    {
        private readonly IMyLogger _logger;

        public BalancyPaymentTracker(IMyLogger logger)
        {
            _logger = logger;
        }

        public void SendItemPurchased(StoreItem item, PurchaseEventArgs eventArgs)
        {
            var paymentInfo = new PaymentInfo
            {
                Currency = eventArgs.purchasedProduct.metadata.isoCurrencyCode,
                Price = (float)eventArgs.purchasedProduct.metadata.localizedPrice,
                ProductId = eventArgs.purchasedProduct.definition.id,
                OrderId = eventArgs.purchasedProduct.transactionID,
                Receipt = eventArgs.purchasedProduct.receipt
            };

            _logger.Log($"[BalancyPaymentValidatorTracker] ValidateAndSendItemPurchased item: {item.Name}");
            SendItemPurchased(item, paymentInfo);
        }

        private void SendItemPurchased(StoreItem storeItem, PaymentInfo paymentInfo)
        {
#if UNITY_IOS
            var platform = Constants.Platform.IosAppStore;
#elif UNITY_ANDROID
            var platform = Constants.Platform.AndroidGooglePlay;
#endif
            LiveOps.Store.ItemWasPurchasedAndValidated(storeItem, paymentInfo, platform, response =>
            {
                if (response.Success)
                {
                    _logger.Log($"[BalancyPaymentValidatorTracker] Successfully purchased. {paymentInfo.ProductId}", DebugStatus.Success);
                }
                else
                {
                    _logger.Log($"[BalancyPaymentValidatorTracker] Purchase failed: {response.Error.Message}", DebugStatus.Fault);
                }
            });
        }
    }
}