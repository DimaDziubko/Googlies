using System;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Data;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using System.Linq;
using _Game.Core.Boosts;

namespace _Game.Gameplay._Boosts.Scripts
{
    public class ProfitOfferBoostCalculator : IDisposable
    {
        private const float MIN_BOOST_VALUE = 1;

        private readonly BoostContainer _boostContainer;
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IShopConfigRepository _config;
        private readonly IMyLogger _logger;

        private IPurchaseDataStateReadonly Purchases => _userContainer.State.PurchaseDataState;

        public ProfitOfferBoostCalculator(
            BoostContainer boostContainer,
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IConfigRepository config,
            IMyLogger logger)
        {
            _boostContainer = boostContainer;
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _config = config.ShopConfigRepository;
            _logger = logger;
            gameInitializer.OnMainInitialization += Init;
        }

        private void Init()
        {
            SubscribeToPurchase();
            UpdateProfitBoost();
        }

        private void SubscribeToPurchase() => Purchases.Changed += OnPurchasesChanged;
        void IDisposable.Dispose()
        {
            Purchases.Changed -= OnPurchasesChanged;
            _gameInitializer.OnMainInitialization -= Init;
        }

        private void OnPurchasesChanged() => UpdateProfitBoost();

        private void UpdateProfitBoost()
        {
            var totalBoost = MIN_BOOST_VALUE;

            var boughtIapIds = Purchases.BoughtIAPs.Select(x => x.IAPId).ToList();
            foreach (var config in _config.GetProfitOfferConfigs())
            {
                if (boughtIapIds.Contains(config.GetProductKey()))
                {
                    totalBoost *= config.CoinBoostFactor;
                }
            }

            _boostContainer.ChangeBoost(BoostSource.Shop, BoostType.CoinsGained, totalBoost);
        }
    }
}