using System;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services._FreeGemsPackService;
using _Game.Core.Services.Audio;
using _Game.Core.UserState._State;
using _Game.UI._Shop.Scripts._ShopScr;
using _Game.UI._Shop.Scripts.Common;
using _Game.Utils.Extensions;
using UnityEngine;
using Zenject;

namespace _Game.UI._Shop.Scripts._FreeGemsPack
{
    public class FreeGemsPackPresenter : IProductPresenter, IDisposable
    {
        public ShopItemView View => _view;

        private readonly FreeGemsPack _gemsPack;
        private readonly FreeGemsPackView _view;

        private readonly ShopIconsContainer _iconsContainer;
        private readonly IAudioService _audioService;
        private readonly IFreeGemsPackService _freeGemsPackService;
        private readonly IMyLogger _logger;
        private readonly IIconConfigRepository _config;

        public FreeGemsPackPresenter(
            FreeGemsPack gemsPack,
            FreeGemsPackView view,
            ShopIconsContainer iconsContainer,
            IAudioService audioService,
            IFreeGemsPackService freeGemsPackService,
            IConfigRepository configRepository,
            IMyLogger logger)
        {
            _gemsPack = gemsPack;
            _view = view;
            _audioService = audioService;
            _iconsContainer = iconsContainer;
            _freeGemsPackService = freeGemsPackService;
            _config = configRepository.IconConfigRepository;
            _logger = logger;
        }

        public void Initialize()
        {
            OnAmountChanged(_gemsPack.Amount);

            Sprite majorIcon = _iconsContainer
                .Get(_config.GetShopIconsReference().AssetGUID)
                .Get(_gemsPack.Config.MajorIconKey);

            _view.SetMajorIcon(majorIcon);

            _view.SetMinorIcon(_config.GetCurrencyIconFor(CurrencyType.Gems));

            _view.Button.HideCurrencyIcon();

            _view.SetQuantity(_gemsPack.Config.Quantity.ToString());

            HandleViewStates();

            _view.Button.ButtonClicked += OnBuyButtonClicked;
            _gemsPack.TimeChanged += OnTimerChanged;
            _gemsPack.IsReadyChanged += OnReadyChanged;
            _gemsPack.AmountChanged += OnAmountChanged;
        }

        private void HandleViewStates()
        {
            if (!_gemsPack.IsReady) HandleRecoveringState();
            else HandleActiveState();
        }

        public void Dispose()
        {
            _view.Button.ButtonClicked -= OnBuyButtonClicked;
            _gemsPack.TimeChanged -= OnTimerChanged;
            _gemsPack.IsReadyChanged -= OnReadyChanged;
            _gemsPack.AmountChanged -= OnAmountChanged;
        }

        private void OnAmountChanged(int amount)
        {
            string info = _gemsPack.Amount <= 1 ? "Free" : _gemsPack.Info;
            _view.Button.SetPrice(info);
        }

        private void OnTimerChanged(float remainingTime)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);
            _view.UpdateTimer(timeSpan.ToCondensedTimeFormat());
        }

        private void HandleActiveState()
        {
            _view.SetActiveTimerView(false);
            _view.Button.SetActive(true);
        }

        private void HandleRecoveringState()
        {
            _view.SetActiveTimerView(true);
            _view.Button.SetActive(false);
        }

        private void OnReadyChanged(bool isReady) => HandleViewStates();

        private void OnBuyButtonClicked()
        {
            _freeGemsPackService.OnFreeGemsPackBtnClicked(_gemsPack);
            _audioService.PlayButtonSound();
        }

        public sealed class Factory : PlaceholderFactory<FreeGemsPack, FreeGemsPackView, FreeGemsPackPresenter>
        {
        }
    }
}