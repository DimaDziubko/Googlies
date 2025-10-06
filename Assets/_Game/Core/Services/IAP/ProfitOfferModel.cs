using System;
using System.Collections.Generic;
using _Game.Core._Reward;
using _Game.Core.Configs.Models._ShopConfig;
using _Game.UI._Shop.Scripts._ShopScr;
using UnityEngine;
using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP
{
    public class ProfitOfferModel : ShopItem
    {
        public string Id;
        public event Action<bool> IsActiveChanged;

        public Sprite MajorIcon;
        public Sprite MinorIcon;

        public ProfitOfferConfig Offer => _offer;
        public int GetCoinBoostFactor() => _offer.CoinBoostFactor;
        public string GetDescription() => _offer.Description;
        public string GetName() => _offer.Name;

        public IReadOnlyList<RewardItemModel> Rewards;
        public bool IsActive { get; private set; }


        public bool IsAvailable;

        private ProfitOfferConfig  _offer;
        private Product _product;
        private int _shopItemViewId;


        public string GetPrice() => _product.metadata.localizedPriceString;

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
            IsActiveChanged?.Invoke(isActive);
        }

        public bool HasReceipt() => _product.hasReceipt;

        public override int ShopItemViewId => _shopItemViewId;
        
        public class ProfitOfferModelBuilder
        {
            string _id;
            Product _product;
            IReadOnlyList<RewardItemModel> _rewards;
            private int _shopItemViewId;
            private Sprite _majorIcon;
            private Sprite _minorIcon;
            private ProfitOfferConfig _offer;
            private bool _isActive;

            public ProfitOfferModelBuilder WithId(string id)
            {
                _id = id;
                return this;
            }

            public ProfitOfferModelBuilder WithMajorIcon(Sprite majorIcon)
            {
                _majorIcon = majorIcon;
                return this;
            }

            public ProfitOfferModelBuilder WithMinorIcon(Sprite minorIcon)
            {
                _minorIcon = minorIcon;
                return this;
            }

            public ProfitOfferModelBuilder WithProduct(Product product)
            {
                _product = product;
                return this;
            }

            public ProfitOfferModelBuilder WithReward(IReadOnlyList<RewardItemModel> reward)
            {
                _rewards = reward;
                return this;
            }

            public ProfitOfferModelBuilder WithView(int shopItemViewId)
            {
                _shopItemViewId = shopItemViewId;
                return this;
            }

            public ProfitOfferModelBuilder IsActive(bool isActive)
            {
                _isActive = isActive;
                return this;
            }

            public ProfitOfferModelBuilder WithOffer(ProfitOfferConfig offer)
            {
                _offer = offer;
                return this;
            }


            public ProfitOfferModel Build()
            {
                return new ProfitOfferModel
                {
                    Id = _id,
                    MajorIcon = _majorIcon,
                    _product = _product,
                    Rewards = _rewards,
                    _shopItemViewId = _shopItemViewId,
                    _offer = _offer,
                    IsActive = _isActive,
                    MinorIcon = _minorIcon
                };
            }

        }
    }
}