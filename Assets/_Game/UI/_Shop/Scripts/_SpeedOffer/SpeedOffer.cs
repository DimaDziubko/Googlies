using _Game.Core.Configs.Models;
using _Game.Core.Configs.Models._ShopConfig;
using _Game.UI._Shop.Scripts._ShopScr;
using UnityEngine.Purchasing;

namespace _Game.UI._Shop.Scripts._SpeedOffer
{
    public class SpeedOffer : ShopItem
    {
        public string Id;
        public Product Product;
        public SpeedBoostOfferConfig Config;
        public override int ShopItemViewId => Config.ShopItemViewId;
    }
}