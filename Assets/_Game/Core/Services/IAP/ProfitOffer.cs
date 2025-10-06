using System;
using _Game.Core.Configs.Models._ShopConfig;
using _Game.UI._Shop.Scripts._ShopScr;
using Sirenix.OdinInspector;
using UnityEngine.Purchasing;

namespace _Game.Core.Services.IAP
{
    public class ProfitOffer : ShopItem
    {
        public event Action<bool> IsActiveChanged;
        
        public string Id;
        public ProfitOfferConfig Config;
        public Product Product;
        
        [ShowInInspector]
        private bool _isActive;
        
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                IsActiveChanged?.Invoke(IsActive);
            }
        }

        public override int ShopItemViewId => Config.ShopItemViewId;
    }
}