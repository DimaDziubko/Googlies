using System;
using System.Collections.Generic;
using System.Linq;

namespace _Game.Core.UserState._State
{
    public interface ICardsCollectionStateReadonly
    {
        event Action<int> CardsSummoningLevelChanged;
        event Action<int> CardsSummoningProgressChanged;
        int CardsSummoningLevel { get; }
        int CardsSummoningProgressCount { get; }
        List<Card> Cards { get; }
    }

    public class CardsCollectionState : ICardsCollectionStateReadonly
    {
        public event Action<int> CardsSummoningLevelChanged;
        public event Action<int> CardsSummoningProgressChanged;

        public int CardSummoningLevel;
        public int CardsSummoningProgressCount;
        public List<Card> Cards;

        int ICardsCollectionStateReadonly.CardsSummoningLevel => CardSummoningLevel;
        int ICardsCollectionStateReadonly.CardsSummoningProgressCount => CardsSummoningProgressCount;
        List<Card> ICardsCollectionStateReadonly.Cards => Cards;


        public void ChangeCardSummoningLevel(int newLevel)
        {
            CardSummoningLevel = newLevel;
            CardsSummoningLevelChanged?.Invoke(CardSummoningLevel);
        }

        public void ChangeCardSummoningProgressCount(int delta)
        {
            CardsSummoningProgressCount += delta;
            CardsSummoningProgressChanged?.Invoke(CardsSummoningProgressCount);
        }
        
        public void AddCard(Card newCard) => Cards.Add(newCard);
    }
}