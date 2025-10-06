using _Game.Core.Boosts;
using System.Collections.Generic;

namespace _Game.UI.Common.Scripts
{
    public interface IProduct
    {
        string Title { get; }
        IReadOnlyList<CurrencyData> Price { get; }
    }
}