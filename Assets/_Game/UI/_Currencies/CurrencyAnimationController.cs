using System;
using _Game.Core.Boosts;
using _Game.Core.UserState._State;
using _Game.Gameplay._Coins.Factory;
using Zenject;

namespace _Game.UI._Currencies
{
    public class CurrencyAnimationController : 
        IInitializable, 
        IDisposable
    {
        private readonly ICurrencyAnimation _animation;
        private readonly CurrencyBank _bank;
        private CurrencyCell CoinsCell => _bank.GetCell(CurrencyType.Coins);
        private CurrencyCell GemsCell => _bank.GetCell(CurrencyType.Gems);
        private CurrencyCell SkillCell => _bank.GetCell(CurrencyType.SkillPotion);
        
        public CurrencyAnimationController(
            ICurrencyAnimation animation,
            CurrencyBank bank)
        {
            _bank = bank;
            _animation = animation;
        }

        public void Initialize()
        {
            CoinsCell.OnAmountAddedFrom += OnCoinsAdded;
            GemsCell.OnAmountAddedFrom += OnGemsAdded;
            SkillCell.OnAmountAddedFrom += OnSkillsPotionAdded;
        }

        public void Dispose()
        {
            CoinsCell.OnAmountAddedFrom -= OnCoinsAdded;
            GemsCell.OnAmountAddedFrom -= OnGemsAdded;
            SkillCell.OnAmountAddedFrom -= OnSkillsPotionAdded;
        }
        private void OnCoinsAdded(double amount, ItemSource source) => 
            _animation.PlayCurrency(CurrencyType.Coins, SelectRenderMode(source));

        private void OnGemsAdded(double amount, ItemSource source) => 
            _animation.PlayCurrency(CurrencyType.Gems, SelectRenderMode(source));

        private void OnSkillsPotionAdded(double amount, ItemSource source) => 
            _animation.PlayCurrency(CurrencyType.SkillPotion, SelectRenderMode(source));

        private CurrencyVfxRenderMode SelectRenderMode(ItemSource source)
        {
            return source switch
            {
                ItemSource.LeaderPass => CurrencyVfxRenderMode.Overlay,
                _ => CurrencyVfxRenderMode.Camera
            };
        }
    }
}