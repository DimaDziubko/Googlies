using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameListenerComposite;
using _Game.Core.Services.Audio;
using _Game.Gameplay.BattleLauncher;
using _Game.UI._AlertPopup;
using _Game.UI.BattleResultPopup.Scripts;
using _Game.UI.Common.Scripts;

namespace _Game.UI._Hud._PauseView
{
    public class BattlePausePresenter : 
        IDisposable, 
        IStartGameListener, 
        IStopGameListener,
        IPauseListener
    {
        private readonly IAudioService _audioService;
        private readonly IAlertPopupProvider _alertPopupProvider;
        private readonly IGameManager _gameManager;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly BattleHud _battleHud;

        private ToggleWithSpriteSwap PauseView => _battleHud.PauseView;
        
        public BattlePausePresenter(
            IAudioService audioService,
            IAlertPopupProvider alertPopupProvider,
            IFeatureUnlockSystem featureUnlockSystem,
            IGameManager gameManager,
            BattleHud battleHud)
        {
            _audioService = audioService;
            _alertPopupProvider = alertPopupProvider;
            _gameManager = gameManager;
            _featureUnlockSystem = featureUnlockSystem;
            _battleHud = battleHud;
            Init();
        }

        private void Init()
        {
            PauseView.ButtonClicked += OnPausedClicked;
            PauseView.SetPaused(_gameManager.IsPaused);
            PauseView.SetActive(false);
        }

        private void OnPausedClicked()
        {
            PauseView.SetInteractable(false);
            _audioService.PlayButtonSound();
            _gameManager.SetPaused(true);
            ShowAlertPopup();
        }

        private async void ShowAlertPopup()
        {
            var popup = await _alertPopupProvider.Load();
            bool isConfirmed = await popup.Value.AwaitForDecision("End battle?");

            if (isConfirmed)
            {
                _gameManager.StopBattle();
                _gameManager.EndBattle(GameResultType.Defeat, true);
            }
            else
            {
                _gameManager.SetPaused(false);
            }
            
            _alertPopupProvider.Dispose();
            PauseView.SetInteractable(true);
        }

        void IPauseListener.SetPaused(bool isPaused) => PauseView.SetPaused(isPaused);
        void IDisposable.Dispose() => PauseView.ButtonClicked -= OnPausedClicked;
        void IStartGameListener.OnStartBattle()
        {
            PauseView.SetActive(_featureUnlockSystem.IsFeatureUnlocked(Feature.Pause));
            PauseView.SetPaused(false);
        }

        void IStopGameListener.OnStopBattle() => PauseView.SetActive(false);
    }
}