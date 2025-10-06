using System;
using _Game.Core.Boosts;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityUtils;

namespace _Game.Core.UserState._State
{
    [Serializable]
    public sealed class CurrencyCell
    {
        public event Action OnStateChanged;
        public event Action<double> OnAmountChanged;
        public event Action<double> OnAmountAdded;
        public event Action<double> OnAmountSpent;
        public event Action<double, ItemSource> OnAmountAddedFrom;
        public event Action<double, ItemSource> OnAmountSpentFrom;
        
        public CurrencyType Type => _data.Type;
        public double Amount => _data.Amount;
        public Sprite Icon => _icon;
        
        [JsonIgnore, SerializeField, PreviewField(75, ObjectFieldAlignment.Left)]
        private Sprite _icon;
        
        [SerializeField] private CurrencyCellData _data;

        public CurrencyCell(CurrencyCellData data)
        {
            _data = data;
        }

        public void Bind(CurrencyCellData data) => 
            _data = data;

        public bool Add(float range, ItemSource source = ItemSource.None)
        {
            if (range <= 0)
            {
                return false;
            }

            _data.Amount += range;
            OnAmountAdded?.Invoke(range);
            OnStateChanged?.Invoke();
            OnAmountAddedFrom?.Invoke(range, source);
            return true;
        }

        public bool Add(double range, ItemSource source = ItemSource.None)
        {
            if (range <= 0)
            {
                return false;
            }

            _data.Amount += range;
            OnAmountAdded?.Invoke(range);
            OnStateChanged?.Invoke();
            OnAmountAddedFrom?.Invoke(range, source);
            return true;
        }

        public bool Spend(float range, ItemSource source = ItemSource.None)
        {
            if (range <= 0)
            {
                return false;
            }

            if (_data.Amount < range)
            {
                return false;
            }

            _data.Amount -= range;
            OnAmountSpent?.Invoke(range);
            OnStateChanged?.Invoke();
            OnAmountSpent?.Invoke(range);
            OnAmountSpentFrom?.Invoke(range, source);
            return true;
        }

        public void Change(float amount)
        {
            if (!amount.Approx((float)_data.Amount))
            {
                _data.Amount = amount;
                OnAmountChanged?.Invoke(_data.Amount);
                OnStateChanged?.Invoke();
            }
        }
        
        public void Change(double amount)
        {
            if (!amount.Approx(_data.Amount))
            {
                _data.Amount = amount;
                OnAmountChanged?.Invoke(_data.Amount);
                OnStateChanged?.Invoke();
            }
        }

        public bool IsEnough(float range)
        {
            return _data.Amount >= range;
        }
    }
}