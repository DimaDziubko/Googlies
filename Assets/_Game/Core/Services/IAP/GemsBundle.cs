using _Game.Core._Reward;
using _Game.UI._Shop.Scripts._ShopScr;
using UnityEngine;
using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP
{
    public class GemsBundle : ShopItem
    {
        public string Id;
        public Product Product;
        public Sprite MajorIcon;
        public RewardItemModel Reward;
        public override int ShopItemViewId => _shopItemViewId;

        private int _shopItemViewId;

        public class GemsBundleBuilder
        {
            string _id;
            Product _product;
            RewardItemModel _reward;
            private int _shopItemViewId;
            private Sprite _majorIcon;

            public GemsBundleBuilder WithId(string id)
            {
                _id = id;
                return this;
            }

            public GemsBundleBuilder WithMajorIcon(Sprite majorIconKey)
            {
                _majorIcon = majorIconKey;
                return this;
            }

            public GemsBundleBuilder WithProduct(Product product)
            {
                _product = product;
                return this;
            }

            public GemsBundleBuilder WithReward(RewardItemModel reward)
            {
                _reward = reward;
                return this;
            }

            public GemsBundleBuilder WithView(int shopItemViewId)
            {
                _shopItemViewId = shopItemViewId;
                return this;
            }


            public GemsBundle Build()
            {
                return new GemsBundle()
                {
                    Id = _id,
                    MajorIcon = _majorIcon,
                    Product = _product,
                    Reward = _reward,
                    _shopItemViewId = _shopItemViewId,
                };
            }

        }
    }
}