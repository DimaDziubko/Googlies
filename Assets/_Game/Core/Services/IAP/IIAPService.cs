using System;
using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP
{
    public interface IIAPService
    {
        event Action<Product> Purchased;
        event Action Restored;
        event Action Initialized;
        event Action<Product> OnEventPurchasedDTD;

        GemsBundleContainer GemsBundleContainer { get; }
        ProfitOfferContainer ProfitOfferContainer { get; }
        void StartPurchase(string productId);
        void UpdateProducts();
        PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs eventArgs);
        void RestorePurchasesAndroid();
        Product GetProductByID(string productID);
        void RestorePurchasesIOS();
        void RegisterPurchaseListener(IPurchaseProcessListener listener);
        void UnregisterPurchaseListener(IPurchaseProcessListener listener);
        void RegisterRestoreListener(IRestoreProcessListener listener);
        void UnregisterRestoreListener(IRestoreProcessListener listener);

        void OnEventPurchasedDTDInvoke(Product productTryToPurchase);
    }

}