using System;
using System.Collections.Generic;
using _Game.UI._CardsGeneral._Cards.Scripts;
using Zenject;

namespace _Game.Gameplay._Cards.Scripts
{
    public class CardShower : IInitializable, IDisposable
    {
        private readonly CardCollector _collector;
        private readonly ICardAppearanceScreenProvider _cardAppearanceScreenProvider;

        public CardShower(
            CardCollector collector,
            ICardAppearanceScreenProvider cardAppearanceScreenProvider)
        {
            _collector = collector;
            _cardAppearanceScreenProvider = cardAppearanceScreenProvider;
        }
        
        void IInitializable.Initialize()
        {
            _collector.CardsCollected += OnCardsAdded;
        }

        void IDisposable.Dispose()
        {
            _collector.CardsCollected -= OnCardsAdded;
        }
        
        private async void OnCardsAdded(List<CardModel> models)
        {
            var cardAppearancePopup = await  _cardAppearanceScreenProvider.Load();
            var isConfirmed = await cardAppearancePopup.Value.ShowAnimationAndAwaitForExit(models);
            if (isConfirmed) _cardAppearanceScreenProvider.Dispose();
        }
    }
}