using System;
using _Game.Core._Reward;
using _Game.Core.Boosts;
using _Game.UI._CardsGeneral._Cards.Scripts;

namespace _Game.Gameplay._RewardProcessing
{
    public class CardItemRewardHandler : IItemRewardHandler
    {
        private readonly CardCollector _cardCollector;
        public CardItemRewardHandler(CardCollector cardCollector)
        {
            _cardCollector = cardCollector;
        }
        
        public void Handle(IRewardItem item, ItemSource source, Action onComplete = null)
        {
            _cardCollector.Collect((int)item.Amount);
        }
    }
}