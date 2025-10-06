using System;
using System.Collections.Generic;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameInitializer;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.Core.Configs.Models._ShopConfig;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._UpgradesScreen.Scripts;
using _Game.UI.Common.Scripts;

namespace _Game.Core.Services.IGPService
{
    public class IGPDto
    {
        public string PurchaseId;
        public string PurchaseType;
        public int PurchaseAmount;
        public int PurchasePrice;
        public string PurchaseCurrency;
        public Dictionary<string, int> Resources;
    }

    public class IGPService : IIGPService, IDisposable
    {
        public event Action<IGPDto> Purchased;

        private readonly IUserContainer _userContainer;
        private readonly IShopConfigRepository _shopConfigRepository;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;
        private readonly CurrencyBank _bank;
        private readonly ShopIconsContainer _iconContainer;
        
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        private List<CoinsBundle> _bundlesCache;
        public List<CoinsBundle> CoinsBundles => _bundlesCache;

        private ProductBuyer _productBuyer;

        public IGPService(
            IUserContainer userContainer,
            IConfigRepository configRepository,
            IFeatureUnlockSystem featureUnlockSystem,
            IGameInitializer gameInitializer,
            CurrencyBank bank,
            ShopIconsContainer container,
            IMyLogger logger)
        {
            _iconContainer = container;
            _bank = bank;
            _userContainer = userContainer;
            _shopConfigRepository = configRepository.ShopConfigRepository;
            _featureUnlockSystem = featureUnlockSystem;
            _gameInitializer = gameInitializer;
            _logger = logger;

            _gameInitializer.OnMainInitialization += Init;
            _bundlesCache = new List<CoinsBundle>();
        }

        private void Init()
        {
            TimelineState.UpgradeItemLevelChanged += OnUpgradeItemChanged;
            TimelineState.NextAgeOpened += OnAgeChanged;
            _featureUnlockSystem.FeatureUnlocked += OnFeatureUnlocked;

            _productBuyer = new ProductBuyer(_bank, _logger);
            _productBuyer.ProductBought += ProductBought;

            UpdateCoinBundles();
        }

        public void StartPurchase(CoinsBundle bundle)
        {
            _productBuyer.Buy(bundle);
        }

        public void Dispose()
        {
            TimelineState.UpgradeItemLevelChanged -= OnUpgradeItemChanged;
            TimelineState.NextAgeOpened -= OnAgeChanged;
            _featureUnlockSystem.FeatureUnlocked -= OnFeatureUnlocked;
            _gameInitializer.OnMainInitialization -= Init;
            _productBuyer.ProductBought -= ProductBought;
        }

        private void ProductBought(IProduct product)
        {
            if (product is CoinsBundle coinsBundle)
            {
                CurrencyData[] reward = new[]
                {
                    coinsBundle.Quantity
                };
                
                _bank.Add(reward);
            }
            else
            {
                _logger.Log($"Wrong type {product.Title}", DebugStatus.Warning);
            }
        }


        private void OnFeatureUnlocked(Feature feature)
        {
            if (feature == Feature.GemsShopping)
            {
                UpdateCoinBundles();
            }
        }

        private void OnAgeChanged() => UpdateCoinBundles();


        private void OnUpgradeItemChanged(UpgradeItemType type, int _)
        {
            if (type == UpgradeItemType.FoodProduction)
            {
                UpdateCoinBundles();
            }
        }

        public void UpdateProducts()
        {
            UpdateCoinBundles();
        }
        
        public void UpdateCoinBundles()
        {
            var message = $"GemsShopping is unlocked {_featureUnlockSystem.IsFeatureUnlocked(Feature.GemsShopping)}";
            _logger.Log(message, DebugStatus.Warning);

            if (!_featureUnlockSystem.IsFeatureUnlocked(Feature.GemsShopping)) return;

            ClearBundles();

            IEnumerable<CoinsBundleConfig> configs = _shopConfigRepository.GetCoinsBundleConfigs();

            foreach (var config in configs)
            {
                var bundle = new CoinsBundle()
                {
                    Id = config.Id,
                    Config = config,
                    Quantity = new CurrencyData()
                    {
                        Type = CurrencyType.Coins,
                        Amount = config.GetQuantity(TimelineState.AgeId,
                            TimelineState.GetUpgradeLevel(UpgradeItemType.FoodProduction)),
                    },
                };

                _bundlesCache.Add(bundle);
            }
        }

        private void ClearBundles() => _bundlesCache.Clear();

        private void Notify(
            string purchaseId,
            string purchaseType,
            int purchaseAmount,
            int purchasePrice,
            string purchaseCurrency,
            Dictionary<string, int> resources = null)
        {
            var dto = new IGPDto()
            {
                PurchaseId = purchaseId,
                PurchaseType = purchaseType,
                PurchaseAmount = purchaseAmount,
                PurchasePrice = purchasePrice,
                PurchaseCurrency = purchaseCurrency,
                Resources = resources
            };

            Purchased?.Invoke(dto);
        }
    }
}