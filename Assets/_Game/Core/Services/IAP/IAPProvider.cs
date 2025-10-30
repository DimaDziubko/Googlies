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
            IUserContainer userContainer
            )
        {
            _userContainer = userContainer;
            _logger = logger;
            _shopConfigRepository = configRepository.ShopConfigRepository;

            _logger.Log("[IAPProvider] Constructor called", DebugStatus.Info);
        }

        public void Initialize(IIAPService iapService)
        {
            _logger.Log("[IAPProvider] Initialize started", DebugStatus.Info);
            _iapService = iapService;

            AllProducts = new Dictionary<string, Product>();

            InitProducts();
        }

        private void InitProducts()
        {
            _logger.Log("[IAPProvider] InitProducts started", DebugStatus.Info);

            if (IsBuilderReady)
            {
                _logger.Log("[IAPProvider] Builder is already ready, skipping rebuild.", DebugStatus.Info);
                return;
            }

            IEnumerable<ProductWrapper> products = _shopConfigRepository.GetProductKeys();

            // Логируем список продуктов
            int productCount = 0;
            _logger.Log("[IAPProvider] Products list:", DebugStatus.Info);
            foreach (var product in products)
            {
                productCount++;
                _logger.Log($"[IAPProvider] Product #{productCount}: ID={product.GetProductKey()}, Type={product.ProductType}", DebugStatus.Info);
            }
            _logger.Log($"[IAPProvider] Total products to initialize: {productCount}", DebugStatus.Info);

            if (productCount == 0)
            {
                _logger.LogWarning("[IAPProvider] No products found to initialize!");
                return;
            }

            try
            {
                _logger.Log("[IAPProvider] Creating ConfigurationBuilder", DebugStatus.Info);
                ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

                // Повторно проходим по продуктам для добавления в builder
                products = _shopConfigRepository.GetProductKeys();
                foreach (var product in products)
                {
                    builder.AddProduct(product.GetProductKey(), product.ProductType);
                }

                IsBuilderReady = true;
                _logger.Log("[IAPProvider] Starting UnityPurchasing.Initialize", DebugStatus.Info);
                UnityPurchasing.Initialize(this, builder);
                _logger.Log("[IAPProvider] UnityPurchasing initialization started.", DebugStatus.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[IAPProvider] Exception during InitProducts: {ex.GetType().Name} - {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        public void StartPurchase(string productId)
        {
            _logger.Log($"[IAPProvider] StartPurchase called for: {productId}", DebugStatus.Info);
            _controller.InitiatePurchase(productId);
        }

        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _logger.Log("[IAPProvider] OnInitialized callback received", DebugStatus.Success);
            _controller = controller;
            _extensions = extensions;

            _logger.Log($"[IAPProvider] Loading {_controller.products.all.Length} products", DebugStatus.Info);
            foreach (Product product in _controller.products.all)
            {
                _logger.Log($"[IAPProvider] Loaded product: {product.definition.id}, Available: {product.availableToPurchase}", DebugStatus.Info);
                AllProducts.Add(product.definition.id, product);
            }

            _logger.Log("[IAPProvider] UnityPurchasing initialization success", DebugStatus.Success);

            OnInitialized?.Invoke();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            _logger.LogError($"[IAPProvider] UnityPurchasing initialization failed: {error}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            _logger.LogError($"[IAPProvider] UnityPurchasing initialization failed: {error}, Message: {message}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            _logger.Log("[IAPProvider] ProcessPurchase started", DebugStatus.Info);

            if (purchaseEvent == null || purchaseEvent.purchasedProduct == null || purchaseEvent.purchasedProduct.definition == null)
            {
                _logger.LogError("[IAPProvider] Invalid purchase event or product.");
                return PurchaseProcessingResult.Complete;
            }

            if (string.IsNullOrEmpty(purchaseEvent.purchasedProduct.definition.id))
            {
                _logger.LogError("[IAPProvider] Product definition ID is null or empty.");
                return PurchaseProcessingResult.Complete;
            }

            _logger.Log($"[IAPProvider] ProcessPurchase success for: {purchaseEvent.purchasedProduct.definition.id}", DebugStatus.Success);

            _productTryToPurchase = purchaseEvent.purchasedProduct;

            bool validPurchase = ValidatePurchase(purchaseEvent);

            _logger.Log($"[IAPProvider] Purchase validation result: {validPurchase}", DebugStatus.Info);

            if (validPurchase)
            {
                _logger.Log($"[IAPProvider] Adding pending purchase: {purchaseEvent.purchasedProduct.definition.id}", DebugStatus.Info);
                _userContainer.PurchaseStateHandler.AddPendingPurchase(purchaseEvent.purchasedProduct.definition.id);
                ProcessDev2DevTracking();
            }
            else
            {
                _logger.LogWarning("[IAPProvider] Purchase validation failed.");
            }

            return _iapService.ProcessPurchase(purchaseEvent);
        }

        private bool ValidatePurchase(PurchaseEventArgs purchaseEvent)
        {
            _logger.Log("[IAPProvider] ValidatePurchase started", DebugStatus.Info);
            bool validPurchase = false;
            bool isValidID = false;

#if !UNITY_EDITOR
    _logger.Log("[IAPProvider] Platform validation (non-editor)", DebugStatus.Info);
    var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
    try
    {
        if (string.IsNullOrEmpty(_productTryToPurchase.receipt))
        {
            _logger.LogError("[IAPProvider] Product receipt is null or empty.");
        }
        else
        {
            _logger.Log("[IAPProvider] Validating receipt", DebugStatus.Info);
            var validationResults = validator.Validate(_productTryToPurchase.receipt);
            foreach (var receipt in validationResults)
            {
                _logger.Log($"[IAPProvider] Checking receipt for productID: {receipt.productID}", DebugStatus.Info);
                if (purchaseEvent.purchasedProduct.definition.storeSpecificId.Equals(receipt.productID))
                {
                    isValidID = true;
                    _logger.Log($"[IAPProvider] Receipt validated for: {receipt.productID}", DebugStatus.Success);
                    break;
                }
            }

            validPurchase = isValidID;
        }
    }
    catch (IAPSecurityException ex)
    {
        _logger.LogError($"[IAPProvider] Receipt validation failed: {ex.Message}");
    }
#endif

#if UNITY_EDITOR
            _logger.Log("[IAPProvider] Editor mode - skipping validation", DebugStatus.Info);
            validPurchase = isValidID = true;
#endif

            _logger.Log($"[IAPProvider] Final validation: validPurchase={validPurchase}, isValidID={isValidID}", DebugStatus.Info);
            return validPurchase && isValidID;
        }

        private void ProcessDev2DevTracking()
        {
            _logger.Log("[IAPProvider] ProcessDev2DevTracking started", DebugStatus.Info);
            try
            {
                var product = _productTryToPurchase;
                string receipt = product.receipt;

                string productID = product.definition.storeSpecificId;
                _logger.Log($"[IAPProvider] Dev2Dev tracking for: {productID}", DebugStatus.Info);
#if UNITY_ANDROID
                _logger.Log("[IAPProvider] Android - starting Dev2Dev verification", DebugStatus.Info);
                DTDAntiCheat.VerifyPayment(IAPGoogleKeyHandler.I.PublicGoogleKey, receipt, result =>
                {
                    _logger.Log($"[IAPProvider] Dev2Dev callback: Status={result.ReceiptStatus}, Result={result.VerificationResult}", DebugStatus.Info);

                    if (result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptValid ||
                        result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptSandbox)
                    {
                        _logger.Log($"[IAPProvider] Dev2Dev AntiCheat: Purchase verified. VerificationResult {result.VerificationResult} ReceiptStatus {result.ReceiptStatus} ", DebugStatus.Success);
                        _iapService.OnEventPurchasedDTDInvoke(product);
                        if (result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptSandbox)
                        {
                            _logger.Log("[IAPProvider] Sandbox purchase detected - setting tester flag", DebugStatus.Info);
                            DTDUserCard.SetTester(true);
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"[IAPProvider] Dev2Dev AntiCheat: Invalid purchase. Status: {result.ReceiptStatus} result {result}");
                    }
                });
#endif
#if UNITY_IOS
                _logger.Log("[IAPProvider] iOS - starting Dev2Dev verification", DebugStatus.Info);
                DTDAntiCheat.VerifyPayment(receipt, result =>
                {
                    _logger.Log($"[IAPProvider] Dev2Dev callback: Status={result.ReceiptStatus}, Result={result.VerificationResult}", DebugStatus.Info);
                    
                    if (result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptValid ||
                            result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptSandbox)
                    {
                        _logger.Log($"[IAPProvider] Dev2Dev AntiCheat: Purchase verified. VerificationResult {result.VerificationResult} ReceiptStatus {result.ReceiptStatus} ", DebugStatus.Success);
                        //_iapService.OnEventPurchasedDTDInvoke(product);
                        if (result.ReceiptStatus == DTDReceiptVerificationStatus.ReceiptSandbox)
                        {
                            _logger.Log("[IAPProvider] Sandbox purchase detected - setting tester flag", DebugStatus.Info);
                            DTDUserCard.SetTester(true);
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"[IAPProvider] Dev2Dev AntiCheat: Invalid purchase. Status: {result.ReceiptStatus} result {result}");
                    }
                });
#endif
            }
            catch (Exception ex)
            {
                _logger.LogError($"[IAPProvider] Exception during Dev2Dev tracking: {ex.GetType().Name} - {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            _logger.LogError($"[IAPProvider] Product {product.definition.id} purchase failed, PurchaseFailureReason {failureReason} transaction id {product.transactionID}");
        }

        void IGameScreenListener<IShopScreen>.OnScreenOpened(IShopScreen screen) { }
        void IGameScreenListener<IShopScreen>.OnInfoChanged(IShopScreen screen) { }
        void IGameScreenListener<IShopScreen>.OnRequiresAttention(IShopScreen screen) { }
        void IGameScreenListener<IShopScreen>.OnScreenClosed(IShopScreen screen) { }
        void IGameScreenListener<IShopScreen>.OnScreenActiveChanged(IShopScreen screen, bool isActive) { }
        void IGameScreenListener<IShopScreen>.OnScreenDisposed(IShopScreen screen) { }
    }
}