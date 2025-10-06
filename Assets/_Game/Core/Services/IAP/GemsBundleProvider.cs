using System.Collections.Generic;
using _Game.Core._IconContainer;
using _Game.Core._Reward;
using _Game.Core.Configs.Repositories;
using _Game.Gameplay._RewardItemResolver;
using UnityEngine;

namespace _Game.Core.Services.IAP
{
    public interface IGemsBundleProvider
    {
        IEnumerable<GemsBundle> GetProducts();
    }

    public class GemsBundleProvider : IGemsBundleProvider
    {
        private readonly IConfigRepository _config;
        private readonly ShopIconsContainer _iconContainer;
        private readonly IAPProvider _iapProvider;
        private readonly RewardItemResolver _resolver;

        public GemsBundleProvider(
            IConfigRepository config,
            ShopIconsContainer iconContainer,
            IAPProvider iapProvider,
            RewardItemResolver resolver)
        {
            _config = config;
            _iconContainer = iconContainer;
            _iapProvider = iapProvider;
            _resolver = resolver;
        }

        public IEnumerable<GemsBundle> GetProducts()
        {
            foreach (var config in _config.ShopConfigRepository.GetGemsBundleConfigs())
            {
                Sprite majorIcon = GetMajorIcon(config.MajorIconKey);
                if (majorIcon == null) continue;

                var rewardItem = RewardItem.CreateDefault();
                rewardItem.Amount = (int)config.Quantity;

                var reward = _resolver.ResolveReward(rewardItem);

                if (_iapProvider.AllProducts.TryGetValue(config.GetProductKey(), out var product))
                {
                    yield return
                        new GemsBundle.GemsBundleBuilder()
                            .WithId(config.GetProductKey())
                            .WithProduct(product)
                            .WithView(config.ShopItemViewId)
                            .WithMajorIcon(majorIcon)
                            .WithReward(reward)
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