using System.Collections.Generic;
using _Game.Core.Configs.Models._Cards;
using _Game.Core.Services.UserContainer;
using _Game.UI._CardsGeneral._Cards.Scripts;

namespace _Game.Core.Configs.Repositories._Cards
{
    public class CardsConfigRepository : ICardsConfigRepository
    {
        private readonly IUserContainer _userContainer;

        public CardsConfigRepository(IUserContainer userContainer) => 
            _userContainer = userContainer;
        
        public bool IsDropListEnabled => _userContainer.GameConfig.SummoningData.DropListsEnabled;
        public List<int> InitialDropList => _userContainer.GameConfig.SummoningData.InitialDropList;
        
        public int MinSummoningLevel => 1;
        public int MaxSummoningLevel => _userContainer.GameConfig.SummoningData.SummoningConfig.Count;

        public int GetX1CardPrice() => _userContainer.GameConfig.CardPricingConfig.x1CardPrice;

        public int GetX10CardPrice() => _userContainer.GameConfig.CardPricingConfig.x10CardPrice;
        public int GetCardsRequiredForNextLevel(int level) => GetSummoningForLevel(level).CardsRequiredForLevel;

        public Dictionary<int, CardsSummoning> GetAllSummonings() => _userContainer.GameConfig.SummoningData.SummoningConfig;
        public bool TryGetCardsByType(CardType type, out List<CardConfig> cards) => 
            _userContainer.GameConfig.CardConfigsByType.TryGetValue(type, out cards);

        public CardConfig ForCard(int cardId) => 
            _userContainer.GameConfig.CardConfigsById[cardId];

        public int GetAllCardsCount() => 
            _userContainer.GameConfig.CardConfigsById.Count;

        public CardsSummoning GetSummoningForLevel(in int summoningLevel) => 
            _userContainer.GameConfig.SummoningData.SummoningConfig[summoningLevel];

        public bool TryGetSummoning(int currentLevel, out CardsSummoning cardsSummoning) => 
            _userContainer.GameConfig.SummoningData.SummoningConfig.TryGetValue(currentLevel, out cardsSummoning);
    }
}