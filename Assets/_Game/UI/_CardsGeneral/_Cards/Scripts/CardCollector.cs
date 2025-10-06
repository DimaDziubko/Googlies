using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.Services.Random;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._Cards.Scripts;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardCollector : IDisposable
    {
        public event Action<List<CardModel>> CardsCollected;
        
        private readonly IGameInitializer _gameInitializer;
        private readonly IUserContainer _userContainer;
        private readonly IConfigRepository _config;
        private readonly CardContainer _cardContainer;

        private ICardsCollectionStateReadonly CardCollection => _userContainer.State.CardsCollectionState;
        private ICardsConfigRepository CardsConfigRepository => _config.CardsConfigRepository;
        
        private readonly CardGenerator _generator;

        public CardCollector(
            IGameInitializer gameInitializer,
            IConfigRepository configRepository,
            IUserContainer userContainer,
            CardContainer cardContainer,
            IRandomService random, 
            IMyLogger logger)
        {
            _cardContainer = cardContainer;
            _userContainer = userContainer;
            _config = configRepository;
            _gameInitializer = gameInitializer;
            
            gameInitializer.OnMainInitialization += Init;
            
            _generator = new CardGenerator(configRepository.CardsConfigRepository, random, logger, userContainer);
        }

        private void Init()
        {
            var modelsToAdd = CreateAndAddCardModels(CardCollection.Cards);
            _cardContainer.Initialize(modelsToAdd);
        }

        private List<CardModel> CreateAndAddCardModels(List<Card> cards)
        {
            var modelsToAdd = new List<CardModel>();
            
            foreach (var card in cards)
            {
                CardModel model = new CardModel(CardsConfigRepository.ForCard(card.Id), card);
                modelsToAdd.Add(model);
            }

            return modelsToAdd;
        }

        public void Collect(int amount)
        {
            List<int> cardIds = _generator.GenerateCards(amount, CardCollection.CardsSummoningLevel);
            
            List<CardModel> collected = new List<CardModel>();
            
            foreach (var id in cardIds)
            {
                if (_cardContainer.Contains(id))
                {
                    CardModel cardModel = _cardContainer.GetById(id).CreateClone();
                    cardModel.SetNew(false);
                    collected.Add(cardModel);
                    _cardContainer.Add(cardModel);
                }
                else
                {
                    Card newCard = new Card
                    {
                        Id = id,
                        Level = 1,
                        Count = 0,
                        Equipped = false,
                        EquippedSlot = -1
                    };
                    
                    CardModel model = new CardModel(CardsConfigRepository.ForCard(newCard.Id), newCard);
                    model.SetNew(true);
                    _userContainer.UpgradeStateHandler.AddCard(newCard);
                    collected.Add(model);
                    _cardContainer.Add(model);
                }

                _userContainer.UpgradeStateHandler.AddSummoningProgressCount(1);
            }
            
            CardsCollected?.Invoke(collected);
        }

        void IDisposable.Dispose()
        {
            _gameInitializer.OnMainInitialization -= Init;
        }
    }
}