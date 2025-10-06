using System.Collections.Generic;
using _Game.Core._IconContainer;
using _Game.Core._Reward;
using _Game.Core.Configs.Repositories;
using _Game.Core.UserState._State;
using _Game.Gameplay._RewardItemResolver;
using UnityEngine;

namespace _Game.Core.Services.IAP
{
    public interface IProfitOfferProvider
    {
        IEnumerable<ProfitOfferModel> GetProducts();
    }

    public class ProfitOfferProvider : IProfitOfferProvider
    {
        private readonly IConfigRepository _config;
        private readonly ShopIconsContainer _iconContainer;
        private readonly IAPProvider _iapProvider;
        private readonly RewardItemResolver _resolver;
        private readonly PurchaseChecker _purchaseChecker;

        public ProfitOfferProvider(
            IConfigRepository config,
            ShopIconsContainer iconContainer,
            IAPProvider iapProvider,
            RewardItemResolver resolver,
            PurchaseChecker purchaseChecker)
        {
            _config = config;
            _iconContainer = iconContainer;
            _iapProvider = iapProvider;
            _resolver = resolver;
            _purchaseChecker = purchaseChecker;
        }

        public IEnumerable<ProfitOfferModel> GetProducts()
        {
            foreach (var config in _config.ShopConfigRepository.GetProfitOfferConfigs())
            {
                Sprite majorIcon = GetMajorIcon(config.MajorIconKey);
                if (majorIcon == null) continue;

                List<RewardItemModel> rewards = new List<RewardItemModel>();

                foreach (var moneyBox in config.MoneyBoxes)
                {
                    var rewardItem = RewardItem.CreateDefault();
                    rewardItem.Amount = (int)moneyBox.Quantity;

                    rewards.Add(_resolver.ResolveReward(rewardItem));
                }

                var minorIcon = _config.IconConfigRepository.GetCurrencyIconFor(CurrencyType.Coins);
                if (majorIcon == null) continue;

                if (_iapProvider.AllProducts.TryGetValue(config.GetProductKey(), out var product))
                {
                    yield return new ProfitOfferModel.ProfitOfferModelBuilder()
                        .WithId(config.GetProductKey())
                        .WithMajorIcon(majorIcon)
                        .WithMinorIcon(minorIcon)
                        .WithReward(rewards)
                        .IsActive(_purchaseChecker.IsBought(config.GetProductKey()))
                        .WithProduct(product)
                        .WithOffer(config)
                        .WithView(config.ShopItemViewId)
                        .Build();

                }
            }

        }

        private Sprite GetMajorIcon(string key)
        {
            return _iconContainer.Get(_config.IconConfigRepository.GetShopIconsReference().AssetGUID)
                .Get(key);
        }
    }
}

