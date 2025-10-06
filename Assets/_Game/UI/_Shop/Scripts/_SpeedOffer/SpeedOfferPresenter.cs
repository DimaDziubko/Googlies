using System;
using System.Globalization;
using _Game._AssetProvider;
using _Game.Core._IconContainer;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services.Audio;
using _Game.Core.Services.IAP;
using _Game.Temp;
using _Game.UI._Shop.Scripts._ShopScr;
using _Game.UI._Shop.Scripts.Common;
using UnityEngine;
using Zenject;

namespace _Game.UI._Shop.Scripts._SpeedOffer
{
    public class SpeedOfferPresenter : IProductPresenter, IDisposable
    {
        public ShopItemView View => _view;
        
        private readonly SpeedOffer _offer;
        private readonly SpeedOfferView  _view;

        private readonly ShopIconsContainer _iconsContainer;
        private readonly IAudioService _audioService;
        private readonly IIAPService _iapService;
        private readonly IIconConfigRepository _iconConfig;

        public SpeedOfferPresenter(
            SpeedOffer offer, 
            SpeedOfferView view,
            ShopIconsContainer iconsContainer,
            IConfigRepository config,
            IAudioService audioService,
            IIAPService iapService)
        {
            _offer = offer;
            _view = view;
            _iconsContainer = iconsContainer;
            _audioService = audioService;
            _iapService = iapService;
            _iconConfig = config.IconConfigRepository;
        }


        public void Initialize()
        {
            Sprite majorIcon = _iconsContainer
                .Get(_iconConfig.GetShopIconsReference().AssetGUID)
                .Get(_offer.Config.MajorIconKey);
            
            _view.SetMajorIcon(majorIcon);
            
            _view.SetDescription(_offer.Config.Description);
            _view.SetValue($"x{_offer.Config.BattleSpeed.SpeedFactor.ToString(CultureInfo.InvariantCulture)}");
            
            _view.Button.HideCurrencyIcon();
        }

        public void Dispose()
        {
            
        }
        
        private void OnInactiveButtonClicked() => 
            GlobalEvents.RaiseOnInsufficientFunds();

        private void OnBuyButtonClicked()
        {
            _iapService.StartPurchase(_offer.Id);
            _audioService.PlayButtonSound();
        }

        public sealed class Factory : PlaceholderFactory<SpeedOffer, SpeedOfferView, SpeedOfferPresenter>
        {
            
        }
    }
}