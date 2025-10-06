using System;
using System.Threading;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.IAP;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.LiveopsCore.Models.ClassicOffer;
using _Game.UI._ParticleAttractorSystem;
using _Game.UI.ClassicOffers.DailyChallenge.Scripts;
using _Game.UI.ClassicOffers.Scripts.Presenter;
using _Game.UI.ClassicOffers.Scripts.Presenters;
using _Game.Utils.Disposable;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityUtils;

namespace _Game.LiveopsCore._EventPresenter
{
    public class ClassicOfferPresenter : GameEventPresenter<ClassicOfferEvent>
    {
        private ClassicOfferPopupProvider _classicOfferPopupProvider;
        private ClassicOfferPopupPresenter _classicOfferPopupPresenter;

        private readonly IIAPService _iapService;
        
        private readonly SemaphoreSlim _popupLock = new(1, 1);

        public ClassicOfferPresenter(
            ClassicOfferEvent gameEvent,
            GameEventView view,
            IMyLogger logger,
            IWorldCameraService cameraService,
            IAudioService audioService,
            CurrencyBank bank,
            IUserContainer userContainer,
            IIconConfigRepository iconConfig,
            IParticleAttractorRegistry particleAttractorRegistry,
            IAPProvider provider)
            : base(gameEvent, view, logger, cameraService, audioService, bank, userContainer, iconConfig, particleAttractorRegistry, provider)
        {

        }

        protected override void OnInitialize()
        {
            if (_event.IsHiden)
            {
                _view.SetActive(false);
                return;
            }

            //_view.InfoView.SetActive(true);
            //_view.ProgressView.SetActive(false);
            //_view.RadialProgressBar.SetActive(false);
            //_view.SetActiveInnerBackground(false);
            //_view.RewardView.SetActive(false);
            //_view.SetInteractable(true);
            //_view.SetLockImageActive(false);
            //_view.SetPinActive(false);

            //_view.ButtonClicked += OnClicked;
            //_event.OnEventTimerTick += OnEventTimerTick;

            //_event.OnSetHiden += DisableView;

            //OnEventTimerTick(_event.EventTimeLeft);
            ////ProgressChanged();

            //SetViewIcon();

            _event.OnOfferPurchased += ProgressChanged;
            //_event.RequiredAttention += OnRequiredAttention;

            ProgressChanged();

            _view.SetCurrencyViewActive(false);

            _view.ButtonClicked += OnClicked;
            _event.OnEventTimerTick += EventTimerTick;
            _event.OnSetHiden += DisableView;
            //_userContainer.State.RaceState.Changed += SetViewIcon;

            _view.SetupPinViewPosition(_event.PanelType);

            _view.SetPinViewActive(false);

            EventTimerTick(_event.EventTimeLeft);
        }

        protected override void OnDispose()
        {
            if (_view.OrNull() != null)
            {
                _view.ButtonClicked -= OnClicked;
            }

            //_userContainer.State.RaceState.Changed -= SetViewIcon;
            _event.OnEventTimerTick -= EventTimerTick;

            _event.OnSetHiden -= DisableView;

            _classicOfferPopupPresenter?.Dispose();

            _logger.Log($"CLASSIC OFFER PRESENTER DISPOSED {GetHashCode()}", DebugStatus.Info);
        }

        private void OnClicked()
        {
            PlayButtonSound();
            
            ShowWithMarks();
        }

        private async UniTask ShowPopup()
        {
            _classicOfferPopupProvider ??= new ClassicOfferPopupProvider(
                _cameraService,
                _audioService);

            Disposable<ClassicOfferPopup> popup = await _classicOfferPopupProvider.Load();

            if (_classicOfferPopupPresenter == null)
            {
                _classicOfferPopupPresenter = new ClassicOfferPopupPresenter(
                    _event,
                    popup.Value,
                    _provider
                );
            }
            else
            {
                _classicOfferPopupPresenter.SetModel(_event);
                _classicOfferPopupPresenter.SetPopup(popup.Value);
            }
            InitPopupValues();

            bool isConfirmed = await popup.Value.ShowAndAwaitForExit();
            if (isConfirmed) popup.Dispose();
        }

        private void InitPopupValues()
        {
            if (_classicOfferPopupPresenter != null)
            {
                _classicOfferPopupPresenter.Initialize();
                _classicOfferPopupPresenter.InitValues();
            }
        }

        private void ProgressChanged()
        {
            _view.SetIcon(_event.Icon);
            _logger.Log("CLASSIC OFFER PROGRESS CHANGED", DebugStatus.Info);

            var info = $"{_event.GetAvalivableCount}/{_event.PurchaseLimit}";

            //_view.ProgressView.SetInfo($"<style=white>{info}</style>");
            //_view.RadialProgressBar.UpdateBar((float)_event.GetAvalivableCount / _event.PurchaseLimit);
        }

        private void EventTimerTick(float seconds)
        {
            var info = $"<style=Green>{TimeSpan.FromSeconds(seconds).ToCondensedTimeFormat()}</style>";

            if (_event.IsLocked) return;

            _view.SetInfo(info);
        }

        private void DisableView()
        {
            if (_view.OrNull() != null)
                _view.SetActive(false);
        }

        private void PlayButtonSound() => _audioService.PlayButtonSound();

        protected override void OnShowRequested()
        {
            ShowWithMarks();
        }
        
        private async void ShowWithMarks()
        {
            if (!await _popupLock.WaitAsync(0)) 
                return;

            try
            {
                _view.SetInteractable(false);
                _event.ShowcaseToken.MarkShown();

                await ShowPopup();

                _event.ShowcaseToken.MarkClosed();
            }
            finally
            {
                _view.SetInteractable(true);
                _popupLock.Release();
            }
        }
    }
}