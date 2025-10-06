using System;
using _Game._AssetProvider;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services._AdsGemsPackService;
using _Game.Core.Services.Audio;
using _Game.Core.UserState._State;
using _Game.UI._Shop.Scripts._ShopScr;
using _Game.UI._Shop.Scripts.Common;
using _Game.UI.Common.Scripts;
using _Game.Utils.Extensions;
using UnityEngine;
using Zenject;

namespace _Game.UI._Shop.Scripts._AdsGemsPack
{
    public class AdsGemsPackPresenter : IProductPresenter, IDisposable
    {
        public ShopItemView View => _view;

        private readonly AdsGemsPack _gemsPack;
        private readonly AdsGemsPackView _view;

        private readonly ShopIconsContainer _iconsContainer;
        private readonly IAudioService _audioService;
        private readonly IAdsGemsPackService _adsGemsPackService;
        private readonly IMyLogger _logger;
        private readonly IIconConfigRepository _config;

        public AdsGemsPackPresenter(
            AdsGemsPack gemsPack,
            AdsGemsPackView view,
            ShopIconsContainer iconsContainer,
            IAudioService audioService,
            IAdsGemsPackService adsGemsPackService,
            IConfigRepository config,
            IMyLogger logger)
        {
            _gemsPack = gemsPack;
            _view = view;
            _iconsContainer = iconsContainer;
            _audioService = audioService;
            _adsGemsPackService = adsGemsPackService;
            _logger = logger;
            _config = config.IconConfigRepository;
        }

        private void Initialize()
        {
            OnAmountChanged(_gemsPack.Amount);

            Sprite majorIcon = _iconsContainer
                .Get(_config.GetShopIconsReference().AssetGUID)
                .Get(_gemsPack.Config.MajorIconKey);

            _view.SetMajorIcon(majorIcon);

            _view.SetMinorIcon(_config.GetCurrencyIconFor(CurrencyType.Gems));

            Sprite adIcon = _config.GetAdsIcon();

            _view.Button.SetCurrencyIcon(adIcon);
            _view.Button.ShowCurrencyIcon();

            _view.SetQuantity(_gemsPack.Config.Quantity.ToString());

            HandleViewStates();

            _gemsPack.TimeChanged += OnTimerChanged;
            _gemsPack.IsLoadedChanged += OnLoaded;
            _gemsPack.IsReadyChanged += OnReadyChanged;
            _gemsPack.AmountChanged += OnAmountChanged;
            _view.Button.ButtonClicked += OnBuyButtonClicked;
        }

        private void OnAmountChanged(int _) => _view.Button.SetPrice(_gemsPack.Info);

        void IProductPresenter.Initialize()
        {
            Initialize();
        }

        private void HandleViewStates()
        {
            if (!_gemsPack.IsLoaded) HandleLoadingState();
            else if (!_gemsPack.IsReady) HandleRecoveringState();
            else HandleActiveState();
        }

        public void Dispose()
        {
            _gemsPack.TimeChanged -= OnTimerChanged;
            _gemsPack.IsLoadedChanged -= OnLoaded;
            _gemsPack.IsReadyChanged -= OnReadyChanged;
            _gemsPack.AmountChanged -= OnAmountChanged;
            _view.Button.ButtonClicked -= OnBuyButtonClicked;
        }

        private void OnTimerChanged(float remainingTime)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);
            _view.UpdateTimer(timeSpan.ToCondensedTimeFormat());
        }

        private void HandleActiveState()
        {
            _view.SetActiveTimerView(false);
            _view.SetActiveLoadingView(false);
            _view.Button.SetActive(true);
        }

        private void HandleRecoveringState()
        {
            _view.SetActiveTimerView(true);
            _view.SetActiveLoadingView(false);
            _view.Button.SetActive(false);
        }

        private void HandleLoadingState()
        {
            _view.SetActiveTimerView(false);
            _view.SetActiveLoadingView(true);
            _view.SetLoadingText("Loading...");
            _view.Button.SetActive(false);
        }

        private void OnReadyChanged(bool isReady) => HandleViewStates();
        private void OnLoaded(bool isLoaded) => HandleViewStates();

        private void OnBuyButtonClicked()
        {
            _adsGemsPackService.OnAdsGemsPackBtnClicked(_gemsPack);
            _audioService.PlayButtonSound();
        }

        public sealed class Factory : PlaceholderFactory<AdsGemsPack, AdsGemsPackView, AdsGemsPackPresenter>
        {
        }
    }
}