using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.Data;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._CardsGeneral._Cards.Scripts;

namespace _Game.Gameplay._Boosts.Scripts
{
    public class CardsBoostCalculator : 
        IDisposable,
        ISaveGameTrigger
    {
        public event Action<bool> SaveGameRequested;

        private const float MIN_BOOST_VALUE = 1;

        private readonly BoostContainer _boostContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly ICardsConfigRepository _cardsConfigRepository;
        private readonly IMyLogger _logger;

        private readonly Dictionary<int, Card> _subscribedCards = new();

        private readonly Dictionary<BoostType, Dictionary<int, float>> _cardBoostsByType = new();
        private readonly CardContainer _cardContainer;
        
        public CardsBoostCalculator(
            BoostContainer boostContainer,
            IGameInitializer gameInitializer,
            IConfigRepository config,
            CardContainer cardContainer,
            IMyLogger logger)
        {
            _cardContainer = cardContainer;
            _boostContainer = boostContainer;
            _gameInitializer = gameInitializer;
            _cardsConfigRepository = config.CardsConfigRepository;
            _logger = logger;
            gameInitializer.OnPostInitialization += Init;

            foreach (BoostType boostType in Enum.GetValues(typeof(BoostType)))
            {
                if (boostType != BoostType.None)
                {
                    _cardBoostsByType[boostType] = new Dictionary<int, float>();
                }
            }
        }

        private void Init()
        {
            _cardContainer.CardAdded += OnCardAdded;
            
            foreach (var card in _cardContainer.GetAll())
            {
                SubscribeToCard(card.Card);
                UpdateCardBoosts(card.Card);
            }
        }

        private void OnCardAdded(CardModel model)
        {
            SubscribeToCard(model.Card);
            UpdateCardBoosts(model.Card);
        }

        private void OnCardsAdded(List<CardModel> models)
        {
            foreach (var model in models)
            {
                SubscribeToCard(model.Card);
                UpdateCardBoosts(model.Card);
            }
        }

        public void Dispose()
        {
            _cardContainer.CardAdded -= OnCardAdded;
            
            _gameInitializer.OnPostInitialization -= Init;
            
            foreach (var card in _subscribedCards.Values)
            {
                card.OnLevelUp -= OnCardsUpgraded;
            }

            _subscribedCards.Clear();
        }

        private void OnCardsUpgraded(Card card)
        {
            UpdateCardBoosts(card);
            SaveGameRequested?.Invoke(false);
        }


        private void UpdateCardBoosts(Card card)
        {
            var config = _cardsConfigRepository.ForCard(card.Id);
            foreach (var boost in config.Boosts)
            {
                if (boost.Type == BoostType.None) continue;

                _cardBoostsByType[boost.Type][card.Id] = boost.Exponential.GetValue(card.Level);
                ApplyBoostTotal(boost.Type);
            }
        }
        
        private void SubscribeToCard(Card card)
        {
            if (_subscribedCards.ContainsKey(card.Id)) return;
            card.OnLevelUp += OnCardsUpgraded;
            _subscribedCards[card.Id] = card;
        }

        private void ApplyBoostTotal(BoostType boostType)
        {
            float totalBoost = MIN_BOOST_VALUE;
            foreach (var boost in _cardBoostsByType[boostType].Values)
            {
                totalBoost *= boost;
            }

            _boostContainer.ChangeBoost(BoostSource.Cards, boostType, totalBoost);
        }
    }
}