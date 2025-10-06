using System;
using _Game.UI.Common.Scripts;

namespace _Game.Gameplay._Buyer
{
    public interface IProductBuyer
    {
        event Action<IProduct> ProductBought;
        bool CanBuy(IProduct product);
        bool Buy(IProduct product);
    }
}
