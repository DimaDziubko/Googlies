using System.Collections.Generic;
using System.Linq;

namespace _Game.Core.UserState._State
{
    public sealed class TemporaryCurrencyBank : BaseCurrencyBank
    {
        public TemporaryCurrencyBank(IEnumerable<CurrencyCell> cells) => 
            _cells = cells.ToDictionary(it => it.Type);
    }
}