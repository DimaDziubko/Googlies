using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Boosts;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.UserState._State
{
    public sealed class CurrencyBank : BaseCurrencyBank, ISaveGameTrigger
    {
        public CurrencyBank(IEnumerable<CurrencyCell> cells) => 
            _cells = cells.ToDictionary(it => it.Type);

        public event Action<bool> SaveGameRequested;
        
        public void Bind(IEnumerable<CurrencyCellData> cells)
        {
            foreach (var cellData in cells)
            {
                if (_cells.TryGetValue(cellData.Type, out var currencyCell))
                {
                    currencyCell.Bind(cellData);
                }
                else
                {
                    _cells[cellData.Type] = new CurrencyCell(new CurrencyCellData {Type = cellData.Type, Amount = 0});
                }
            }
        }

        public override void Add(IEnumerable<CurrencyData> range)
        {
            base.Add(range);
            SaveGameRequested?.Invoke(true);
        }

        public override void Add(CurrencyData currency)
        {
            base.Add(currency);
            SaveGameRequested?.Invoke(true);
        }

        public override bool Spend(IEnumerable<CurrencyData> range)
        {
            var success = base.Spend(range);
            if (success) SaveGameRequested?.Invoke(true);
            return success;
        }
    }
}