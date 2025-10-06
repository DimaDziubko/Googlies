using System.Collections.Generic;
using _Game.Core.Boosts;
using _Game.UI._UpgradesScreen.Scripts;
using _Game.UI.Common.Scripts;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsBundle : IProduct
    {
        public string Title => "Cards bundle";

        public IReadOnlyList<CurrencyData> Price => _currencyData;
        public int Quantity => _quantity;
        public ItemSource Source => _source;

        private readonly CurrencyData[] _currencyData;

        private readonly int _quantity;
        private readonly ItemSource _source;


        public CardsBundle(int quantity, CurrencyData[] currencyData, ItemSource cardsSource)
        {
            _currencyData = currencyData;
            _quantity = quantity;
            _source = cardsSource;
        }
    }
}