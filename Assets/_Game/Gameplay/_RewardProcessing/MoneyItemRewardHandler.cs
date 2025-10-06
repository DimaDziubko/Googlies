using System;
using _Game.Core._Logger;
using _Game.Core._Reward;
using _Game.Core.Boosts;
using _Game.Core.UserState._State;

namespace _Game.Gameplay._RewardProcessing
{
    public class MoneyItemRewardHandler : IItemRewardHandler
    {
        private readonly CurrencyBank _bank;
        private readonly IMyLogger _logger;

        public MoneyItemRewardHandler( 
            CurrencyBank bank,
            IMyLogger logger)
        {
            _logger = logger;
            _bank = bank;
        }
        public void Handle(IRewardItem rewardItem, ItemSource source, Action onComplete = null)
        {
            _logger.Log($"Adding money item reward {rewardItem.Id}, amount: {rewardItem.Amount} source: {source}", DebugStatus.Info);
            
            if (rewardItem.Details is MoneyItemLocal moneyItem)
            {
                _bank.Add(new CurrencyData() 
                {
                    Amount = rewardItem.Amount,
                    Source = source,
                    Type = moneyItem.CurrencyType
                });
            }
        }
    }
}