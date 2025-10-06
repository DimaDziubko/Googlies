using System;
using _Game.Core._Logger;
using _Game.Core.UserState._State;
using _Game.UI.Common.Scripts;
using Sirenix.OdinInspector;

namespace _Game.UI._UpgradesScreen.Scripts
{
    public sealed class ProductBuyer
    {
        public event Action<IProduct> ProductBought;

        private readonly CurrencyBank _currencyCurrencyBank;
        private readonly IMyLogger _logger;

        public ProductBuyer(CurrencyBank currencyBank, IMyLogger logger)
        {
            _currencyCurrencyBank = currencyBank;
            _logger = logger;
        }

        [Button]
        public bool CanBuy(IProduct product)
        {
            return product == null
                ? throw new ArgumentNullException(nameof(product))
                : _currencyCurrencyBank.IsEnough(product.Price);
        }
        
        [Button]
        public bool Buy(IProduct product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            if (!_currencyCurrencyBank.IsEnough(product.Price))
            {
                _logger.Log($"<color=red>Not enough money for product {product.Title}!</color>", DebugStatus.Warning);
                return false;
            }

            _currencyCurrencyBank.Spend(product.Price);
            ProductBought?.Invoke(product);

            _logger.Log($"<color=green>Product {product.Title} successfully purchased!</color>", DebugStatus.Warning);
            return true;
        }
    }
}