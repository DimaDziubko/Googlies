using System;
using _Game.Core.UserState._State;
using Sirenix.OdinInspector;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardPresenter
    {
        private CardModel _cardModel;
        private CardView _view;

        [ShowInInspector]
        public bool NeedIlluminationAnimation => _cardModel.IsNew || _cardModel.IsHighest;
        
        [ShowInInspector]
        public bool IsNew => _cardModel.IsHighest;
        
        [ShowInInspector]
        public bool IsHighestCardType => _cardModel.IsHighest;
        

        public CardPresenter(CardModel cardModel, CardView view)
        {
            _cardModel = cardModel;
            _view = view;
        }

        public void Initialize()
        {
            _cardModel.Card.OnLevelUp += OnLevelUp;
            _cardModel.NewMarkChanged += OnNewMarkChanged;
            
            _view.SetColor(_cardModel.GetColor(), _cardModel.GetMaterial());
            _view.SetIcon(_cardModel.Icon);
            _view.SetNew(_cardModel.IsNew);
            
            OnLevelUp(_cardModel.Card);
        }

        public void PlaySimpleAppearanceAnimation()
        {
            _view.SetActive(true);
            _view.Show();
            _view.PlaySimpleAppearanceAnimation();
        }

        public void PlayRippleAppearanceAnimation(Action callback)
        {
            _view.SetActive(true);
            _view.Show();
            _view.PlayRippleAppearanceAnimation(callback);
        }

        private void OnLevelUp(Card card) => 
            _view.SetLevel($"Level {card.Level.ToString()}");

        public void Dispose()
        {
            _cardModel.NewMarkChanged -= OnNewMarkChanged;
            _cardModel.Card.OnLevelUp -= OnLevelUp;
        }
        
        private void OnNewMarkChanged()
        {
            _view.SetNew(false);
        }
    }
}