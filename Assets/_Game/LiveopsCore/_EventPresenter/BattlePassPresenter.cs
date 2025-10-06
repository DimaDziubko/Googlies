using System;
using System.Threading;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.IAP;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.LiveopsCore.Models.BattlePass;
using _Game.UI._ParticleAttractorSystem;
using _Game.UI.BattlePass.Scripts;
using _Game.Utils.Disposable;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityUtils;

namespace _Game.LiveopsCore._EventPresenter
{
    public class BattlePassPresenter : GameEventPresenter<BattlePassEvent>
    {
        private IBattlePassPopupProvider _battlePassPopupProvider;
        private BattlePassPopupPresenter _battlePassPopupPresenter;

        private Disposable<BattlePassPopup> _popup;

        private readonly SemaphoreSlim _popupLock = new(1, 1);
        
        public BattlePassPresenter(BattlePassEvent gameEvent,
            GameEventView view,
            IMyLogger logger,
            IWorldCameraService cameraService,
            IAudioService audioService,
            CurrencyBank bank,
            IUserContainer userContainer,
            IIconConfigRepository iconConfig,
            IParticleAttractorRegistry particleAttractorRegistry,
            IAPProvider provider) :
            base(gameEvent, view, logger, cameraService, audioService, bank, userContainer, iconConfig, particleAttractorRegistry, provider)
        {
        }

        protected override void OnInitialize()
        {
            _logger.Log("BATTLE PASS ONINITIALIZED", DebugStatus.Info);
            
            _event.ProgressTextChanged += OnProgressTextChanged;
            _event.RequiredAttention += OnRequiredAttention;
            
            OnProgressTextChanged();
            
            _view.SetCurrencyViewActive(true);

            _view.ButtonClicked += OnClicked;
            _event.EventTimerTick += EventTimerTick;
            
            _view.SetupPinViewPosition(_event.PanelType);
            
            _view.SetPinViewActive(_event.NeedAttention());
            
            _view.SetInteractable(true);
            
            EventTimerTick(_event.EventTimeLeft);
        }

        private void OnProgressTextChanged()
        {
            _view.CurrencyView.SetIcon(_iconConfig.GetCurrencyIconFor(_event.CurrencyCellType));
            _view.CurrencyView.SetupCurrency(_event.ProgressText);
        }

        protected override void OnDispose()
        {
            _logger.Log("BATTLE PASS ONDISPOSED", DebugStatus.Info);
            
            _event.ProgressTextChanged -= OnProgressTextChanged;
            _event.RequiredAttention -= OnRequiredAttention;
            
            _battlePassPopupProvider?.Dispose();
            
            if (_view.OrNull() != null) 
                _view.ButtonClicked -= OnClicked;
            
            _event.EventTimerTick -= EventTimerTick;
        }

        private void OnRequiredAttention()
        {
            _logger.Log($"BP Required Attention {_event.NeedAttention()}", DebugStatus.Info);
            
            if (_view.OrNull() != null) 
                _view.SetPinViewActive(_event.NeedAttention());
        }

        private void OnClicked()
        {
            PlayButtonSound();
            ShowWithMarks();
        }

        private async UniTask ShowPopup()
        {
            _logger.Log("SHOW BATTLE PASS POPUP", DebugStatus.Info);
            
            if(_popup != null) return;
            
            if (_battlePassPopupPresenter == null)
                _battlePassPopupPresenter = new BattlePassPopupPresenter(_event, _iconConfig, _provider, _logger);
            else
                _battlePassPopupPresenter.SetModel(_event);
            
            _battlePassPopupProvider ??= new BattlePassPopupProvider(
                _battlePassPopupPresenter,
                _audioService,
                _logger);

            _popup = await _battlePassPopupProvider.Load();
            
            bool isConfirmed = await _popup.Value.AwaitForExit();
            if (isConfirmed) _battlePassPopupProvider.Dispose();
        }

        private void EventTimerTick(float seconds)
        {
            var info = $"<style=Green>{TimeSpan.FromSeconds(seconds).ToCondensedTimeFormat()}</style>";
            
            if (_event.IsLocked) return;

            _view.SetInfo(info);
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
                _popup = null;
            }
            finally
            {
                _view.SetInteractable(true);
                _popupLock.Release();
            }
        }
    }
}