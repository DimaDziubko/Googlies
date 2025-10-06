using System;
using _Game.Core._DataProviders._FoodDataProvider;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay.Food.Scripts
{
    public sealed class FoodContainer : IFoodContainer
    {
        public event Action OnStateChanged;
        public event Action<int> OnAmountChanged;
        public event Action<int> OnAmountAdded;
        public event Action<int> OnAmountSpent;
        public event Action<float> OnProgressChanged;
        public int Amount => _amount;
        public float Progress => _progress;
        
        public Sprite FoodIcon => _provider.GetData().FoodIcon;
        public float ProductionSpeed => _provider.GetData().ProductionSpeed;
        
        [ShowInInspector]
        private int _amount;
        private float _progress;
        
        private readonly IFoodProductionProvider _provider;

        public FoodContainer(IFoodProductionProvider provider)
        {
            _provider = provider;
            Reset();
        }

        public bool Add(int range)
        {
            if (range <= 0)
            {
                return false;
            }

            _amount += range;
            OnAmountAdded?.Invoke(range);
            OnStateChanged?.Invoke();
            return true;
        }

        public bool Spend(int range)
        {
            if (range <= 0)
            {
                return false;
            }

            if (_amount < range)
            {
                return false;
            }

            _amount -= range;
            OnAmountSpent?.Invoke(range);
            OnStateChanged?.Invoke();
            return true;
        }

        public void Change(int amount)
        {
            if (_amount != amount)
            {
                _amount = amount;
                OnAmountChanged?.Invoke(_amount);
                OnStateChanged?.Invoke();
            }
        }
        
        public void SetProgress(float progress)
        {
            if (Math.Abs(_progress - progress) > 0.01f)
            {
                _progress = progress;
                OnProgressChanged?.Invoke(_progress);
            }
        }

        public void Clear()
        {
            _amount = 0;
            _progress = 0;
        }

        public void Reset()
        {
            _amount = _provider.GetData().InitialFoodAmount;
            _progress = 0f;
            OnAmountChanged?.Invoke(_amount);
            OnProgressChanged?.Invoke(_progress);
            OnStateChanged?.Invoke();
        }

        public bool IsEnough(float range)
        {
            return _amount >= range;
        }
    }
}