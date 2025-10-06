using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._RewardItemResolver;
using _Game.Gameplay._RewardProcessing;
using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP
{
    public class IAPService : IIAPService, IDisposable
    {
        public event Action Initialized;
        public event Action Restored;
        public event Action<Product> Purchased;
        public event Action<Product> OnEventPurchasedDTD;

        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IAPProvider _iapProvider;
        private readonly IIconConfigRepository _config;
        private readonly IMyLogger _logger;

        private IPurchaseDataStateReadonly PurchaseData => _userContainer.State.PurchaseDataState;

        public bool IsInitialized => _iapProvider.IsInitialized;
        public bool IsRestored => PurchaseData.Restored;

        public GemsBundleContainer GemsBundleContainer => _gemsBundleContainer;
        public ProfitOfferContainer ProfitOfferContainer => _profitOfferContainer;

        private readonly GemsBundleContainer _gemsBundleContainer;
        private IGemsBundleProvider _gemsBundleProvider;
        private GemsBundlePurchaseProcessor _gemsBundleProcessor;

        private readonly ProfitOfferContainer _profitOfferContainer;
        private IProfitOfferProvider _profitOfferProvider;
        private ProfitOfferProcessor _profitOfferProcessor;

        private readonly IAPRestoreProcessor _iapRestoreProcessor;
        private readonly ShopIconsContainer _iconContainer;

        private List<IPurchaseProcessListener> _purchaseProcessListeners = new();
        private List<IRestoreProcessListener> _restoreProcessListeners = new();

        public IAPService(
            IAPProvider iapProvider,
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IConfigRepository configRepository,
            ShopIconsContainer iconsContainer,
            RewardItemResolver resolver,
            RewardProcessingService rewardProcessingService,
            PurchaseChecker purchaseChecker,
            IMyLogger logger)
        {
            _config = configRepository.IconConfigRepository;
            _userContainer = userContainer;
            _iapProvider = iapProvider;
            _gameInitializer = gameInitializer;
            _logger = logger;
            _iconContainer = iconsContainer;
            
            _gemsBundleContainer = new GemsBundleContainer();
            _profitOfferContainer = new ProfitOfferContainer();

            _gemsBundleProvider = new GemsBundleProvider(configRepository, _iconContainer, _iapProvider, resolver);
            _gemsBundleProcessor = new GemsBundlePurchaseProcessor(_gemsBundleContainer, rewardProcessingService, userContainer);

            _profitOfferProvider = new ProfitOfferProvider(configRepository, _iconContainer, _iapProvider, resolver, purchaseChecker);
            _profitOfferProcessor = new ProfitOfferProcessor(_profitOfferContainer, _userContainer, rewardProcessingService, _logger, purchaseChecker);

            _iapRestoreProcessor = new IAPRestoreProcessor(_iapProvider, _logger, _userContainer);

            RegisterPurchaseListener(_gemsBundleProcessor);
            RegisterPurchaseListener(_profitOfferProcessor);
            RegisterRestoreListener(_profitOfferProcessor);

            _gameInitializer.OnMainInitialization += Init;
        }

        private void Init()
        {
            _iapProvider.Initialize(this);
            _iapProvider.OnInitialized += () => Initialized?.Invoke();
            PurchaseData.Changed += RefreshProducts;

            RefreshProducts();
        }

        void IDisposable.Dispose()
        {
            UnregisterPurchaseListener(_gemsBundleProcessor);
            UnregisterPurchaseListener(_profitOfferProcessor);
            UnregisterRestoreListener(_profitOfferProcessor);

            _gameInitializer.OnMainInitialization -= Init;
            PurchaseData.Changed -= RefreshProducts;
        }

        void IIAPService.StartPurchase(string productId) =>
            _iapProvider.StartPurchase(productId);

        void IIAPService.UpdateProducts()
        {
            RefreshProducts();
            CheckAndDispatchPendingPurchases();
        }

        public void RegisterPurchaseListener(IPurchaseProcessListener listener)
        {
            if (!_purchaseProcessListeners.Contains(listener))
            {
                _purchaseProcessListeners.Add(listener);
            }
        }

        public void UnregisterPurchaseListener(IPurchaseProcessListener listener)
        {
            if (_purchaseProcessListeners.Contains(listener))
            {
                _purchaseProcessListeners.Remove(listener);
            }
        }

        public void RegisterRestoreListener(IRestoreProcessListener listener)
        {
            if (!_restoreProcessListeners.Contains(listener))
            {
                _restoreProcessListeners.Add(listener);
            }
        }
        public void UnregisterRestoreListener(IRestoreProcessListener listener)
        {
            if (_restoreProcessListeners.Contains(listener))
            {
                _restoreProcessListeners.Remove(listener);
            }
        }

        private void RefreshProducts()
        {
            foreach (GemsBundle product in _gemsBundleProvider.GetProducts())
            {
                if (!_gemsBundleContainer.Contains(product.Id))
                {
                    _gemsBundleContainer.AddOrUpdate(product.Id, product);
                }
            }

            DefineProfitOffers();
        }

        private void DefineProfitOffers()
        {
            foreach (ProfitOfferModel product in _profitOfferProvider.GetProducts())
            {
                if (!_profitOfferContainer.Contains(product.Id))
                {
                    _profitOfferContainer.AddOrUpdate(product.Id, product);
                }
            }
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs eventArgs)
        {
            CheckOrDefineNonConsumableProducts();

            if (!PurchaseData.Restored)
                OnRestoreComplete();

            if (eventArgs == null) return PurchaseProcessingResult.Complete;
            if (eventArgs.purchasedProduct == null) return PurchaseProcessingResult.Complete;

            foreach (var listener in _purchaseProcessListeners)
                listener.ProcessPurchase(eventArgs);

            Purchased?.Invoke(eventArgs.purchasedProduct);
            return PurchaseProcessingResult.Complete;
        }

        public void OnEventPurchasedDTDInvoke(Product productTryToPurchase)
        {
            OnEventPurchasedDTD?.Invoke(productTryToPurchase);
        }

        private void CheckAndDispatchPendingPurchases()
        {
            foreach (var pendingIaP in PurchaseData.PendingIAPs)
            {
                foreach (var listener in _purchaseProcessListeners)
                {
                    listener.ProcessPendingPurchase(pendingIaP);
                }
            }
        }

        void IIAPService.RestorePurchasesAndroid() => _iapRestoreProcessor.RestorePurchasesAndroid(OnRestoreComplete);

        Product IIAPService.GetProductByID(string productID) =>
            _iapProvider.AllProducts.GetValueOrDefault(productID);

        void IIAPService.RestorePurchasesIOS() => _iapRestoreProcessor.RestorePurchasesIOS(OnRestoreComplete);

        private void CheckOrDefineNonConsumableProducts() => DefineProfitOffers();

        private void OnRestoreComplete()
        {
            CheckOrDefineNonConsumableProducts();

            foreach (var listener in _restoreProcessListeners)
            {
                listener.OnRestore();
            }

            _userContainer.State.PurchaseDataState.Restore();
            Restored?.Invoke();
        }
    }
}