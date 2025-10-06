using System;
using System.Globalization;
using _Game.Core.Services.Audio;
using _Game.Core.Services.IAP;
using _Game.UI._Shop.Scripts._ShopScr;
using _Game.UI._Shop.Scripts.Common;
using Zenject;

namespace _Game.UI._Shop.Scripts._GemsBundle
{
    public class GemsBundlePresenter : IProductPresenter, IDisposable
    {
        public ShopItemView View => _view;
        
        private GemsBundle _bundle;
        private GemsBundleView _view;
        
        private readonly IAudioService _audioService;
        private readonly IIAPService _iapService;

        public GemsBundlePresenter(
            GemsBundle bundle, 
            GemsBundleView view,
            IAudioService audioService,
            IIAPService iapService)
        {
            _bundle = bundle;
            _view = view;
            _audioService = audioService;
            _iapService = iapService;
        }
        
        public void Initialize()
        {
            _view.SetQuantity(_bundle.Reward.Amount.ToString(CultureInfo.InvariantCulture));

            var majorIcon = _bundle.MajorIcon;
            if(majorIcon != null)
                _view.SetMajorIcon(majorIcon);
            
            var minorIcon = _bundle.Reward.Icon;
            if(minorIcon != null)
                _view.SetMinorIcon(minorIcon);

            _view.Button.SetActive(true);
            _view.Button.SetInteractable(true);
            _view.Button.SetPrice(_bundle.Product.metadata.localizedPriceString);
            _view.Button.HideCurrencyIcon();
            
            _view.Button.ButtonClicked += OnBuyButtonClicked;
        }

        public void Dispose() => _view.Button.ButtonClicked -= OnBuyButtonClicked;

        private void OnBuyButtonClicked()
        {
            _iapService.StartPurchase(_bundle.Id);
            _audioService.PlayButtonSound();
        }

        public sealed class Factory : PlaceholderFactory<GemsBundle, GemsBundleView, GemsBundlePresenter>
        {
            
        }
    }
}