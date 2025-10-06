using System.Collections.Generic;
using _Game.Core.Boosts;
using _Game.UI.Common.Scripts;

namespace _Game.UI._Skills.Scripts
{
    public class SkillBundle : IProduct
    {
        public string Title => "Skill bundle";

        public IReadOnlyList<CurrencyData> Price => _currencyData;
        public int Quantity => _quantity;

        private readonly CurrencyData[] _currencyData;
        
        private readonly int _quantity;
        public SkillBundle(int quantity, CurrencyData[] currencyData)
        {
            _currencyData = currencyData;
            _quantity = quantity;
        }
    }
}