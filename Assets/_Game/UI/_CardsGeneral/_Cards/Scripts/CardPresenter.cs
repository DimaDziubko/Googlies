using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.UserState._State;
using _Game.Gameplay._Boosts.Scripts;
using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardPresenter
    {
        private CardModel _cardModel;
        private CardView _view;
        private readonly IIconConfigRepository _iconConfig;

        [ShowInInspector]
        public bool NeedIlluminationAnimation => _cardModel.IsNew || _cardModel.IsHighest;

        [ShowInInspector]
        public bool IsNew => _cardModel.IsHighest;

        [ShowInInspector]
        public bool IsHighestCardType => _cardModel.IsHighest;


        public CardPresenter(CardModel cardModel, CardView view, IIconConfigRepository iconConfigRepository)
        {
            _cardModel = cardModel;
            _view = view;
            _iconConfig = iconConfigRepository;
        }

        public void Initialize()
        {
            _cardModel.Card.OnLevelUp += OnLevelUp;
            _cardModel.NewMarkChanged += OnNewMarkChanged;
            _view.SetColor(_cardModel.GetColor(), _cardModel.GetMaterial());
            _view.SetIcon(_cardModel.Icon);
            _view.SetNew(_cardModel.IsNew);

            InitBoostView(_cardModel.Boosts);

            OnLevelUp(_cardModel.Card);
        }

        private void InitBoostView(Boost[] boosts)
        {
            if (boosts.Length > 0 & boosts.Length < 2)
                _view.SetBoostView(GetBoostSprite(boosts[0].Type));
            else if (boosts.Length >= 2)
                _view.SetBoostView(GetBoostSprite(boosts[0].Type), GetBoostSprite(boosts[1].Type));
        }

        private Sprite GetBoostSprite(BoostType type)
        {
            //UnityEngine.Debug.Log($"Try To Get Boost Type {type}");
            return _iconConfig.GetBoostIconCardsFor(type);
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
            _view.SetLevel($"Lv.{card.Level.ToString()}");

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