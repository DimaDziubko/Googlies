using System;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Models._ShopConfig;
using _Game.UI._Shop.Scripts._ShopScr;

namespace _Game.UI._Shop.Scripts._AdsGemsPack
{
    public class AdsGemsPack : ShopItem
    {
        public event Action<bool> IsReadyChanged;
        public event Action<float> TimeChanged;
        public event Action<bool> IsLoadedChanged;
        public event Action<int> AmountChanged;
        
        public int Id;
        public AdsGemsPackConfig Config;
        public string Info => $"{_amount}/{Config.DailyGemsPackCount}";

        private bool _isReady;
        private float _remainingTime;
        private bool _isLoaded;
        private int _amount;

        public float RemainingTime => _remainingTime;
        
        public int Amount => _amount;
        public bool IsLoaded => _isLoaded;
        public bool IsReady => _isReady;

        public void SetLoaded(bool isLoaded)
        {
            _isLoaded = isLoaded;
            IsLoadedChanged?.Invoke(_isLoaded);
        }

        public void SetAmount(int amount)
        {
            if (_amount == amount)
            {
                return;
            }

            _amount = amount;
            AmountChanged?.Invoke(_amount); 
            SetReady(_amount > 0);
        }

        private void SetReady(bool isReady)
        {
            _isReady = isReady;
            IsReadyChanged?.Invoke(isReady);
        }

        public void Tick(float remainingTime)
        {
            _remainingTime = remainingTime;
            TimeChanged?.Invoke(_remainingTime);
        }

        public override int ShopItemViewId => Config.ShopItemViewId;
    }
}