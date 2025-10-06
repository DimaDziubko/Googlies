using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Configs.Models._Cards;
using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.Services.Random;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.UI._CardsGeneral._Cards.Scripts;

namespace _Game.Gameplay._Cards.Scripts
{
    public class CardGenerator
    {
        private readonly ICardsConfigRepository _cardsConfigRepository;
        private readonly IRandomService _random;
        private readonly ICardsScreenPresenter _cardsScreenPresenter;
        private readonly IMyLogger _logger;
        private readonly IUserContainer _userContainer;

        private ICardsCollectionStateReadonly CardsState => _userContainer.State.CardsCollectionState;
        
        public CardGenerator(
            ICardsConfigRepository cardsConfigRepository,
            IRandomService random,
            IMyLogger logger,
            IUserContainer userContainer)
        {
            _cardsConfigRepository = cardsConfigRepository;
            _random = random;
            _logger = logger;
            _userContainer = userContainer;
        }

        public List<int> GenerateCards(int amount, int summoningLevel)
        {
            List<int> cardsId = new List<int>(amount);
            
            int initialDropListCount = _cardsConfigRepository.InitialDropList.Count;
            
            for (int i = 0; i < amount; i++)
            {
                var type = SelectType(summoningLevel);

                int currentIndex = CardsState.CardsSummoningProgressCount + i;
                
                _logger.Log($"currentIndex {currentIndex}  initialDropListCount {initialDropListCount}");
                if (currentIndex < initialDropListCount)
                {
                    _logger.Log($"Get card from initial drop list", DebugStatus.Info);
                    int cardId = _cardsConfigRepository.InitialDropList[currentIndex];
                    cardsId.Add(cardId);
                }
                else
                {
                    if (_cardsConfigRepository.TryGetCardsByType(type, out List<CardConfig> cardsCollection))
                    {
                        var randomIndex = _random.Next(0, cardsCollection.Count);
                        cardsId.Add(cardsCollection[randomIndex].Id);
                    }
                }
            }

            foreach (var id in cardsId)
            {
                _logger.Log($"GENERATED CARD WITH ID: {id}");
            }

            return cardsId;
        }

        private CardType SelectType(int summoningLevel)
        {
            CardsSummoning summoning = _cardsConfigRepository.GetSummoningForLevel(summoningLevel);

            float totalChance = 100;
            float randomPoint = _random.Next(0, totalChance);

            float currentSum = 0;

            var dropChances = new List<(CardType Type, float Chance)>()
            {
                (CardType.Common, summoning.Common),
                (CardType.Rare, summoning.Rare),
                (CardType.Epic, summoning.Epic),
                (CardType.Legendary, summoning.Legendary)
            };

            foreach (var chance in dropChances)
            {
                currentSum += chance.Chance;
                if (randomPoint <= currentSum)
                {
                    return chance.Type;
                }
            }

            return CardType.Common;
        }
    }
}