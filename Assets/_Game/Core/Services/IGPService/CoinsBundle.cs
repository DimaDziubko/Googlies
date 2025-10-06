using System;
using System.Collections.Generic;
using _Game.Core.Boosts;
using _Game.Core.Configs.Models._ShopConfig;
using _Game.Core.UserState._State;
using _Game.UI._Shop.Scripts._ShopScr;
using _Game.UI.Common.Scripts;

namespace _Game.Core.Services.IGPService
{
    public class CoinsBundle : ShopItem, IProduct
    {
        public event Action<CurrencyData> QuantityChanged;
        public string Title => $"Coins bundle {Id}";

        public int Id;
        public CoinsBundleConfig Config;

        private CurrencyData _quantity;
        private bool _isAffordable;
        
        public CurrencyData Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                QuantityChanged?.Invoke(_quantity);
            }
        }
        
        public override int ShopItemViewId => Config.ShopItemViewId;
        
        public IReadOnlyList<CurrencyData> Price => new[]
        {
            new CurrencyData
            {
                Type = CurrencyType.Gems,
                Amount = Config.Price,
                Source = ItemSource.CoinsBundle
            }
        };
    }
}