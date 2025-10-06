using System;
using System.Globalization;
using _Game.Core.Services.Audio;
using _Game.Core.Services.IAP;
using _Game.UI._Shop.Scripts._AmountView;
using _Game.UI._Shop.Scripts._ShopScr;
using _Game.UI._Shop.Scripts.Common;
using UnityEngine;
using Zenject;

namespace _Game.UI._Shop.Scripts._ProfitOffer
{
    public class ProfitOfferPresenter : IProductPresenter, IDisposable
    {
        public ShopItemView  View => _view;
        
        private readonly ProfitOfferModel _offer;
        private readonly ProfitOfferView  _view;
        
        private readonly IAudioService _audioService;
        private readonly IIAPService _iapService;

        public ProfitOfferPresenter(
            ProfitOfferModel offer,
            ProfitOfferView view,
            IAudioService audioService,
            IIAPService iapService)
        {
            _offer = offer;
            _view = view;
            
            _audioService = audioService;
            _iapService = iapService;
        }

        public void Initialize()
        {
            Sprite majorIcon = _offer.MajorIcon;
            
            if (majorIcon != null)
                _view.SetMajorIcon(majorIcon);
            
            Sprite minorIcon = _offer.MinorIcon;
            
            if (minorIcon != null)
                _view.SetMinorIcon(minorIcon);
            

            foreach (var reward in _offer.Rewards)
            {
                AmountView element = _view.SpawnResourceElement();

                element.SetIcon(reward.Icon);
                element.SetAmount(reward.Amount.ToString(CultureInfo.InvariantCulture));
                element.Show();
            }

            _view.SetDescription($"x{_offer.GetCoinBoostFactor()} {_offer.GetDescription()}");
            _view.SetValue($"x{_offer.GetCoinBoostFactor()}");
            _view.SetName(_offer.GetName());
            _view.SetActive(_offer.IsActive);
            
            _view.Button.SetInteractable(true);
            _view.Button.SetPrice(_offer.GetPrice());
            _view.Button.HideCurrencyIcon();

            _offer.IsActiveChanged += OnActiveChanged;
            _view.Button.ButtonClicked += OnBuyButtonClicked;
        }

        public void Dispose()
        {
            _offer.IsActiveChanged -= OnActiveChanged;
            _view.Button.ButtonClicked -= OnBuyButtonClicked;
        }

        private void OnActiveChanged(bool isActive) => _view.SetActive(isActive, true);

        private void OnBuyButtonClicked()
        {
            _iapService.StartPurchase(_offer.Id);
            _audioService.PlayButtonSound();
        }

        public sealed class Factory : PlaceholderFactory<ProfitOfferModel, ProfitOfferView, ProfitOfferPresenter>
        {
            
        }
    }
}