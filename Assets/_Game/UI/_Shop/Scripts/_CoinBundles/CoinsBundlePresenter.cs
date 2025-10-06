using System;
using System.Linq;
using _Game.Core._IconContainer;
using _Game.Core.Boosts;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services.Audio;
using _Game.Core.Services.IGPService;
using _Game.Core.UserState._State;
using _Game.Temp;
using _Game.UI._Shop.Scripts._ShopScr;
using _Game.UI._Shop.Scripts.Common;
using _Game.Utils.Extensions;
using UnityEngine;
using Zenject;

namespace _Game.UI._Shop.Scripts._CoinBundles
{
    public class CoinsBundlePresenter : IProductPresenter, IDisposable
    {
       public ShopItemView View => _view;
        
        private readonly CoinsBundle _bundle;
        private readonly CoinsBundleView _view;
        
        private readonly IAudioService _audioService;
        private readonly IIGPService _igpService;
        private readonly IIconConfigRepository _iconConfig;
        private readonly ShopIconsContainer _iconsContainer;

        private readonly CurrencyBank _bank;
        private CurrencyCell Cell => _bank.GetCell(CurrencyType.Gems);

        public CoinsBundlePresenter(
            CoinsBundle bundle, 
            CoinsBundleView view,
            CurrencyBank bank,
            ShopIconsContainer iconsContainer,
            IAudioService audioService,
            IIGPService igpService,
            IConfigRepository config)
        {
            _bank = bank;
            _bundle = bundle;
            _view = view;
            _audioService = audioService;
            _igpService = igpService;
            _iconConfig = config.IconConfigRepository;
            _iconsContainer = iconsContainer;
        }
        
        public void Initialize()
        {
            OnQuantityChanged(_bundle.Quantity);
            
            _view.SetCurrencyIcon(_iconConfig.GetCurrencyIconFor(CurrencyType.Gems));
            
            Sprite majorIcon = _iconsContainer
                .Get(_iconConfig.GetShopIconsReference().AssetGUID)
                .Get(_bundle.Config.MajorIconKey);
            
            _view.SetMajorIcon(majorIcon);
            
            _view.SetMinorIcon(_iconConfig.GetCurrencyIconFor(CurrencyType.Coins));

            OnCurrencyStateChanged();

            _view.Button.SetActive(true);
            _view.Button.SetPrice(_bundle.Price.FirstOrDefault().Amount.ToCompactFormat());
            
            _bundle.QuantityChanged += OnQuantityChanged;

            _view.Button.ButtonClicked += OnBuyButtonClicked;
            _view.Button.InactiveClicked += OnInactiveButtonClicked;

            Cell.OnStateChanged += OnCurrencyStateChanged;
        }

        public void Dispose()
        {
            _bundle.QuantityChanged -= OnQuantityChanged;

            _view.Button.ButtonClicked -= OnBuyButtonClicked;
            _view.Button.InactiveClicked -= OnInactiveButtonClicked;
            
            Cell.OnStateChanged -= OnCurrencyStateChanged;
        }

        private void OnCurrencyStateChanged()
        {
            _view.Button.SetInteractable(_bank.IsEnough(_bundle.Price));
        }

        private void OnQuantityChanged(CurrencyData newValue) => 
            _view.SetQuantity(newValue.Amount.ToCompactFormat());

        private void OnInactiveButtonClicked() => 
            GlobalEvents.RaiseOnInsufficientFunds();

        private void OnBuyButtonClicked()
        {
            _igpService.StartPurchase(_bundle);
            _audioService.PlayButtonSound();
        }

        public sealed class Factory : PlaceholderFactory<CoinsBundle, CoinsBundleView, CoinsBundlePresenter>
        {
            
        }
    }
}