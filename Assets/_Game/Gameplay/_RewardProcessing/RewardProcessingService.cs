using System;
using System.Collections.Generic;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core._Logger;
using _Game.Core._Reward;
using _Game.Core.Boosts;
using _Game.Core.UserState._State;
using _Game.UI._CardsGeneral._Cards.Scripts;

namespace _Game.Gameplay._RewardProcessing
{
    public class RewardProcessingService : IDisposable
    {
        private readonly Dictionary<Type, IItemRewardHandler> _handlers = new();
        
        private readonly IMyLogger _logger;

        public RewardProcessingService(
            IMyLogger logger,
            IDungeonModelFactory dungeonModelFactory,
            CurrencyBank bank,
            CardCollector carCollector)
        {
            _logger = logger;
            
            _handlers.Add(typeof(MoneyItemLocal), new MoneyItemRewardHandler(bank, logger));
            _handlers.Add(typeof(CardItemLocal), new CardItemRewardHandler(carCollector));
            _handlers.Add(typeof(KeyItemLocal), new KeyItemRewardHandler(dungeonModelFactory));
        }

        public void Process(IRewardItem item, ItemSource source, Action onComplete = null)
        {
            Type itemType = item.Details.GetType();

            if (_handlers.TryGetValue(itemType, out var handler))
            {
                handler.Handle(item, source, onComplete);
            }
            else
            {
                _logger.LogError($"No reward handler registered for item type {itemType.Name}");
            }
        }
        
        void IDisposable.Dispose()
        {

        }
    }
}