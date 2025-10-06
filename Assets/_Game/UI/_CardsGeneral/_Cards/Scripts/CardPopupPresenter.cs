using System;
using System.Collections;
using _Game.Core.UserState._State;
using UnityEngine;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardPopupPresenter
    {
        public event Action StateChanged;
        
        private CardModel _model;

        public CardPopupPresenter(CardModel model)
        {
            _model = model;
        }

        public void Initialize()
        {
            _model.Card.OnLevelUp += OnLevelUp;
            _model.Card.CountChanged += OnCountChanged;
        }

        public void Dispose()
        {
            _model.Card.OnLevelUp -= OnLevelUp;
            _model.Card.CountChanged -= OnCountChanged;
        }
        
        public CardModel Model => _model;
        public bool IsReadyForUpgrade => _model.IsReadyForUpgrade;
        public int Level => _model.Card.Level;

        public string GetName() => _model.Name;
        public string GetTypeName() => _model.Type.ToString();
        public string GetDescription() => _model.CardDescription;

        public void Upgrade()
        {
            if(!IsReadyForUpgrade) return;
            _model.Upgrade();
        }

        private void OnCountChanged() => StateChanged?.Invoke();
        private void OnLevelUp(Card card) => StateChanged?.Invoke();
        public Color GetTypeColor() => _model.GetColor();
        public IEnumerable GetBoosts() => _model.Boosts;
    }
}