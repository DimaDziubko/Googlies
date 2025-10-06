using System;
using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._ShopConfig;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using DevToDev.Analytics;
using DevToDev.AntiCheat;
using UnityEngine;
using UnityEngine.Purchasing;
#if !UNITY_EDITOR
using UnityEngine.Purchasing.Security;
#endif

namespace _Game.Core.Services.IAP
{
    public class IAPProvider :
        IStoreListener,
        IGameScreenListener<IShopScreen>
    {
        public bool IsBuilderReady;

        private readonly IShopConfigRepository _shopConfigRepository;
        private readonly IMyLogger _logger;

        private IStoreController _controller;
        private IExtensionProvider _extensions;

        public IExtensionProvider Extensions => _extensions;

        private IIAPService _iapService;
        private Product _productTryToPurchase;
        private readonly IUserContainer _userContainer;

        public Dictionary<string, Product> AllProducts { get; private set; }

        public event Action OnInitialized;
        public bool IsInitialized => _controller != null && _extensions != null && IsBuilderReady;

        public IAPProvider(
            IMyLogger logger,
            IConfigRepository configRepository,
            IUserContainer userContainer)
        {
            _userContainer = userContainer;
            _logger = logger;
            _shopConfigRepository = configRepository.ShopConfigRepository;
        }

        public void Initialize(IIAPService iapService)
        {
            _iapService = iapService;

            AllProducts = new Dictionary<string, Product>();
            
            InitProducts();
        }

        private void InitProducts()
        {
            if (IsBuilderReady)
            {
                _logger.Log("OnProductsRefreshed: Builder is already ready, skipping rebuild.", DebugStatus.Info);
                return;
            }

            IEnumerable<ProductWrapper> products = _shopConfigRepository.GetProductKeys();

            try
            {
                ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

                foreach (var product in products)
                {
                    builder.AddProduct(product.GetProductKey(), product.ProductType);
                }

                IsBuilderReady = true;
                UnityPurchasing.Initialize(this, builder);
                _logger.Log("OnProductsRefreshed: UnityPurchasing initialization started.", DebugStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError($"OnProductsRefreshed: Exception during build - {ex}.");
            }
        }

        public void StartPurchase(string productId) =>
            _controller.InitiatePurchase(productId);

        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _controller = controller;
            _extensions = extensions;

            foreach (Product product in _controller.products.all)
            {
                _logger.Log($"[IAP] Загрузился продукт: {product.definition.id}");
                AllProducts.Add(product.definition.id, product);
            }

            _logger.Log("UnityPurchasing initialization success");

            OnInitialized?.Invoke();
        }

        public void OnInitializeFailed(InitializationFailureReason error) =>
            _logger.LogError($"UnityPurchasing initialization failed {error}");

        public void OnInitializeFailed(InitializationFailureReason error, string message) =>
            _logger.LogError($"UnityPurchasing initialization failed {error}, {message}");

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            if (purchaseEvent == null || purchaseEvent.purchasedProduct == null || purchaseEvent.purchasedProduct.definition == null)
            {
                _logger.LogError("Invalid purchase event or product.");
                return PurchaseProcessingResult.Complete;
            }

            if (string.IsNullOrEmpty(purchaseEvent.purchasedProduct.definition.id))
            {
                _logger.LogError("Product definition ID is null or empty.");
                return PurchaseProcessingResult.Complete;
            }

            _logger.Log($"UnityPurchasing ProcessPurchase success {purchaseEvent.purchasedProduct.definition.id}", DebugStatus.Success);

            _productTryToPurchase = purchaseEvent.purchasedProduct;

            bool validPurchase = ValidatePurchase(purchaseEvent);

            _logger.Log($"Is purchase valid {validPurchase}", DebugStatus.Info);

            if (validPurchase)
            {
                _userContainer.PurchaseStateHandler.AddPendingPurchase(purchaseEvent.purchasedProduct.definition.id);
                ProcessByteBrewTracking();
                ProcessDev2DevTracking();
            }
            else
            {
                Debug.LogWarning("Purchase validation failed.");
            }

            return _iapService.ProcessPurchase(purchaseEvent);
        }

        private bool ValidatePurchase(PurchaseEventArgs purchaseEvent)
        {
            bool validPurchase = false;
            bool isValidID = false;

#if !UNITY_EDITOR
    var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
    try
    {
        if (string.IsNullOrEmpty(_productTryToPurchase.receipt))
        {
            Debug.LogError("Product receipt is null or empty.");
        }
        else
        {
            var validationResults = validator.Validate(_productTryToPurchase.receipt);
            foreach (var receipt in validationResults)
            {
                if (purchaseEvent.purchasedProduct.definition.storeSpecificId.Equals(receipt.productID))
                {
                    isValidID = true;
                    break;
                }
            }

            validPurchase = isValidID;
        }
    }
    catch (IAPSecurityException ex)
    {
        Debug.LogError("Receipt validation failed: " + ex.Message);
    }
#endif

#if UNITY_EDITOR
            validPurchase = isValidID = true;
#endif

            return validPurchase && isValidID;
        }

        private void ProcessByteBrewTracking()
        {
            try
            {
                string receipt = _productTryToPurchase.receipt;
                string currency = _productTryToPurchase.metadata.isoCurrencyCode;
                int amount = (int)_productTryToPurchase.metadata.localizedPrice;
                string productID = _productTryToPurchase.definition.storeSpecificId;

                var receiptWrapper = MiniJson.JsonDecode(receipt) as Dictionary<string, object>;
                if (receiptWrapper != null && receiptWrapper.TryGetValue("Payload", out var payload))
                {
#if UNITY_ANDROID
                    var payloadData = MiniJson.JsonDecode(payload.ToString()) as Dictionary<string, object>;
#endif
#if UNITY_IOS
                    // ByteBrew.ValidateiOSInAppPurchaseEvent("Apple App Store", currency, amount, productID, "IapShop", payload.ToString(),
                    //     purchaseResultData =>
                    //     {
                    //         Debug.Log($"ByteBrew Purchase Valid: {purchaseResultData.isValid}, Message: {purchaseResultData.message}");
                    //     });
#endif
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception during ByteBrew tracking: " + ex);
            }
        }

        private void ProcessDev2DevTracking()
        {
            try
            {
                var product = _productTryToPurchase;
                string receipt = product.receipt;

                string productID = product.definition.storeSpecificId;
#if UNITY_ANDROID

                DTDAntiCheat.VerifyPayment(IAPGoogleKeyHandler.I.PublicGoogleKey, receipt, result =>
                {
                    //result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptInternalError ||
                    //result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptServerError ||
                    if (result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptValid ||
                        result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptSandbox)
                    {
                        _logger.Log($"Dev2Dev AntiCheat: Purchase verified. VerificationResult {result.VerificationResult} ReceiptStatus {result.ReceiptStatus} ");
                        //TODO: Check
                        _iapService.OnEventPurchasedDTDInvoke(product);
                        if (result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptSandbox)
                            DTDUserCard.SetTester(true);
                    }
                    else
                    {
                        _logger.LogWarning($"Dev2Dev AntiCheat: Invalid purchase. Status: {result.ReceiptStatus} result {result}");
                    }
                });
#endif
#if UNITY_IOS
                DTDAntiCheat.VerifyPayment(receipt, result =>
                {
                    if (result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptValid ||
                            result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptSandbox)
                    {
                        _logger.Log($"Dev2Dev AntiCheat: Purchase verified. VerificationResult {result.VerificationResult} ReceiptStatus {result.ReceiptStatus} ");
                        //_iapService.OnEventPurchasedDTDInvoke(product);
                        if (result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptSandbox)
                            DTDUserCard.SetTester(true);
                    }
                    else
                    {
                        _logger.LogWarning($"Dev2Dev AntiCheat: Invalid purchase. Status: {result.ReceiptStatus} result {result}");
                    }
                });
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception during Dev2Dev tracking: " + ex);
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) =>
            _logger.LogError($"Product {product.definition.id} purchase failed, PurchaseFailureReason {failureReason} transaction id {product.transactionID}");

        void IGameScreenListener<IShopScreen>.OnScreenOpened(IShopScreen screen) { }
        void IGameScreenListener<IShopScreen>.OnInfoChanged(IShopScreen screen) { }
        void IGameScreenListener<IShopScreen>.OnRequiresAttention(IShopScreen screen) { }
        void IGameScreenListener<IShopScreen>.OnScreenClosed(IShopScreen screen) { }
        void IGameScreenListener<IShopScreen>.OnScreenActiveChanged(IShopScreen screen, bool isActive) { }
        void IGameScreenListener<IShopScreen>.OnScreenDisposed(IShopScreen screen) { }
    }
}