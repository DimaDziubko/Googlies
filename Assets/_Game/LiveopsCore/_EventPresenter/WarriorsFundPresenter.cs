using System.Threading;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.IAP;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.LiveopsCore.Models.WarriorsFund;
using _Game.UI._ParticleAttractorSystem;
using _Game.UI.WarriorsFund.Scripts;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityUtils;

namespace _Game.LiveopsCore._EventPresenter
{
    public class WarriorsFundPresenter : GameEventPresenter<WarriorsFundEvent>
    {
        private WarriorsFundPopupProvider _warriorsFundPopupProvider;
        private WarriorsFundPopupPresenter _warriorsFundPopupPresenter;

        private Disposable<WarriorsFundPopup> _popup;

        private readonly SemaphoreSlim _popupLock = new(1, 1);
        
        public WarriorsFundPresenter(WarriorsFundEvent gameEvent,
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
            _logger.Log("WARRIORS FUND ONINITIALIZED", DebugStatus.Info);
            
            _event.RequiredAttention += OnRequiredAttention;
            
            
            _view.SetCurrencyViewActive(false);
            _view.SetInfoViewActive(false);

            _view.ButtonClicked += OnClicked;
            
            _view.SetupPinViewPosition(_event.PanelType);
            
            _view.SetPinViewActive(_event.NeedAttention());
            
            _view.SetInteractable(true);
            
        }
        
        protected override void OnDispose()
        {
            _logger.Log("WARRIORS FUND ONDISPOSED", DebugStatus.Info);
            
            _event.RequiredAttention -= OnRequiredAttention;
            
            _warriorsFundPopupProvider?.Dispose();
            
            if (_view.OrNull() != null) 
                _view.ButtonClicked -= OnClicked;
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
            
            if (_warriorsFundPopupPresenter == null)
                _warriorsFundPopupPresenter = new WarriorsFundPopupPresenter(_event, _provider, _logger);
            else
                _warriorsFundPopupPresenter.SetModel(_event);
            
            _warriorsFundPopupProvider ??= new WarriorsFundPopupProvider(
                _warriorsFundPopupPresenter,
                _audioService,
                _logger);

            _popup = await _warriorsFundPopupProvider.Load();
            
            bool isConfirmed = await _popup.Value.AwaitForExit();
            if (isConfirmed) _warriorsFundPopupProvider.Dispose();
        }
        
        private void PlayButtonSound() => _audioService.PlayButtonSound();

        protected override void OnShowRequested() => ShowWithMarks();

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