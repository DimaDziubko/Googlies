using System;
using System.Collections;
using System.Collections.Generic;
using _Game.Core.Boosts;
using Sirenix.OdinInspector;

namespace _Game.Core.UserState._State
{
    public abstract class BaseCurrencyBank : IEnumerable<CurrencyCell>
    {
        [ShowInInspector, ReadOnly]
        protected Dictionary<CurrencyType, CurrencyCell> _cells = new();
        
        public virtual CurrencyCell GetCell(in CurrencyType type) => _cells[type];

        public virtual bool IsEnough(IEnumerable<CurrencyData> range)
        {
            foreach (var currency in range)
            {
                if (!_cells.TryGetValue(currency.Type, out var cell))
                    throw new ArgumentException($"Currency type {currency.Type} is not found!");

                if (!cell.IsEnough(currency.Amount))
                    return false;
            }

            return true;
        }

        public virtual void Add(IEnumerable<CurrencyData> range)
        {
            foreach (var currency in range)
                GetCell(currency.Type).Add(currency.Amount, currency.Source);
        }

        public virtual void Add(CurrencyData currency) =>
            GetCell(currency.Type).Add(currency.Amount, currency.Source);

        public virtual bool Spend(IEnumerable<CurrencyData> range)
        {
            if (!IsEnough(range)) return false;

            foreach (var currency in range)
                GetCell(currency.Type).Spend(currency.Amount, currency.Source);

            return true;
        }

        public virtual bool HasCell(CurrencyType type) => _cells.ContainsKey(type);

        public virtual void AddCell(CurrencyCell cell)
        {
            if (!HasCell(cell.Type))
                _cells[cell.Type] = cell;
        }

        public IEnumerator<CurrencyCell> GetEnumerator() => _cells.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}