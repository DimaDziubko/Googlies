using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.UserState._State;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.Utils._Static;
using UnityEngine;

namespace _Game.UI._CardsGeneral._Summoning.Scripts
{
    public class SummoningPopupPresenter
    {
        private ICardsCollectionStateReadonly _cardsState;
        private ICardsConfigRepository _cardsConfigRepository;

        public SummoningPopupPresenter(
            ICardsCollectionStateReadonly cardsState,
            ICardsConfigRepository cardsConfigRepository)
        {
            _cardsState = cardsState;
            _cardsConfigRepository = cardsConfigRepository;
        }

        public int Level => _cardsState.CardsSummoningLevel;

        public bool CanMoveNext(in int levelToShow) => levelToShow < _cardsConfigRepository.MaxSummoningLevel;
        public bool CanMovePrevious(in int levelToShow) => levelToShow > _cardsConfigRepository.MinSummoningLevel;
        public Color GetColorFor(CardType type) => CardColorMapper.GetColorForType(type);


        public string GetSummoningValueFor(CardType type, in int level) => 
            _cardsConfigRepository.GetSummoningForLevel(level).ForType(type);
    }
}