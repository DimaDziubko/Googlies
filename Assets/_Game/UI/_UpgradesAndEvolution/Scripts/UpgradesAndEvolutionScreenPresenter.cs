using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.Audio;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._BoostPopup;
using _Game.UI._EvolveScreen.Scripts; 
using _Game.UI._MainMenu.State;
using _Game.UI._TravelScreen.Scripts;
using _Game.UI._UpgradesScreen.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityUtils;

namespace _Game.UI._UpgradesAndEvolution.Scripts
{
    public class UpgradesAndEvolutionScreenPresenter :
        IUpgradeUnitsScreen,
        IGameScreenEvents<IUpgradeUnitsScreen>,
        IGameScreenListener<IUpgradesScreen>,
        IGameScreenListener<IEvolveScreen>,
        IGameScreenListener<IMenuScreen>,
        IDisposable
    {
        public event Action<IUpgradeUnitsScreen> ScreenOpened;
        public event Action<IUpgradeUnitsScreen> InfoChanged;
        public event Action<IUpgradeUnitsScreen> RequiresAttention;
        public event Action<IUpgradeUnitsScreen> ScreenClosed;
        public event Action<IUpgradeUnitsScreen, bool> ActiveChanged;
        public event Action<IUpgradeUnitsScreen> ScreenDisposed;

        [ShowInInspector, ReadOnly]
        public bool IsReviewed => _screenStateAggregator.IsReviewed;
        
        [ShowInInspector, ReadOnly]
        public bool NeedAttention => _screenStateAggregator.NeedAttention;
        
        // public UpgradeAndEvolutionScreen Screen { get; set; }

        private readonly IUpgradesScreenProvider _upgradesScreenProvider;
        private readonly IUINotifier _uiNotifier;
        private readonly IAudioService _audioService;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly ITutorialManager _tutorialManager;
        private readonly IEvolveScreenProvider _evolveScreenProvider;
        private readonly ITravelScreenProvider _travelScreenProvider;
        private readonly IMyLogger _logger;
        private readonly IAgeNavigator _ageNavigator;
        private ITimelineNavigator _timelineNavigator;

        private LocalStateMachine _localStateMachine;

        private readonly QuickBoostInfoPresenter.Factory _factory;
        private QuickBoostInfoPresenter _presenter;


        [ShowInInspector, ReadOnly]
        private readonly ScreenStateAggregator _screenStateAggregator;

        private bool _openEvolveScreenFirst;

        public UpgradesAndEvolutionScreenPresenter(
            IUpgradesScreenProvider upgradesScreenProvider,
            IUINotifier uiNotifier,
            IAudioService audioService,
            IFeatureUnlockSystem featureUnlockSystem,
            ITutorialManager tutorialManager,
            IEvolveScreenProvider evolveScreenProvider,
            ITravelScreenProvider travelScreenProvider,
            QuickBoostInfoPresenter.Factory factory,
            IMyLogger logger,
            IAgeNavigator ageNavigator,
            ITimelineNavigator timelineNavigator)
        {
            _upgradesScreenProvider = upgradesScreenProvider;
            _uiNotifier = uiNotifier;
            _audioService = audioService;
            _featureUnlockSystem = featureUnlockSystem;
            _tutorialManager = tutorialManager;
            _factory = factory;
            _evolveScreenProvider = evolveScreenProvider;
            _travelScreenProvider = travelScreenProvider;
            _logger = logger;
            _ageNavigator = ageNavigator;
            _screenStateAggregator = new ScreenStateAggregator();
            _timelineNavigator = timelineNavigator;
            
            _uiNotifier.RegisterScreen(this, this);
        }

        // async void IUpgradesAndEvolutionScreenPresenter.OnUpgradesAndEvolutionScreenOpened()
        // {
        //     if (Screen.OrNull() != null)
        //     {
        //         _tutorialManager.Register(Screen.EvolutionStep);
            
        //         ScreenOpened?.Invoke(this);
                
        //         if (_featureUnlockSystem.IsFeatureUnlocked(Feature.EvolutionScreen))
        //             Screen.EvolutionStep.ShowStep();
            
        //         Unsubscribe();
        //         Subscribe();
        //         await InitStateMachine();
        //         InitButtons();
        //         InitQuickBoostInfo();
            
        //             OnUpgradesButtonClicked();
        //     }
        // }

        // void IUpgradesAndEvolutionScreenPresenter.OnScreenActiveChanged(bool isActive)
        // {
        //     _localStateMachine?.SetActive(isActive);
        //     ActiveChanged?.Invoke(this, isActive);
        // }

        // void IUpgradesAndEvolutionScreenPresenter.OnUpgradesAndEvolutionScreenClosed()
        // {
        //     _logger.Log("upgradesAndEvolutionScreenPresenter CLOSED", DebugStatus.Warning);
        //     Screen.EvolutionStep.CancelStep();
        //     _tutorialManager.UnRegister(Screen.EvolutionStep);
            
        //     _presenter?.Dispose();
        //     _localStateMachine?.Exit();
            
        //     _uiNotifier.UnregisterPin(typeof(IUpgradesScreen));
        //     _uiNotifier.UnregisterPin(typeof(IEvolveScreen));
            
        //     Unsubscribe();
            
        //     ScreenClosed?.Invoke(this);
        // }
        
        private void InitQuickBoostInfo()
        {
            // if (_presenter != null)
            // {
            //     _presenter.Dispose();
            //     _presenter.SetView(Screen.QuickBoostInfoPanel);
            // }
            // else
            // {           
            //     _presenter = _factory.Create(Screen.QuickBoostInfoPanel);
            // }

            // _presenter.Initialize();
        }

        private void Subscribe()
        {
            // Screen.UpgradesButton.ButtonClicked += OnUpgradesButtonClicked;
        }

        private void Unsubscribe()
        {
            // if (Screen != null)
            // {
            //     Screen.UpgradesButton.ButtonClicked -= OnUpgradesButtonClicked;
            // }
        }
        
        private void InitButtons()
        {
            // Screen.UpgradesButton.UnHighlight();
            // Screen.UpgradesButton.SetLocked(false);
            // Screen.UpgradesButton.SetInteractable(true);
            // _uiNotifier.RegisterPin(typeof(IUpgradesScreen), Screen.UpgradesButton.Pin);

            // bool isEvolutionUnlocked = _featureUnlockSystem.IsFeatureUnlocked(Feature.EvolutionScreen);
            // Screen.EvolutionButton.UnHighlight();
            // Screen.EvolutionButton.SetLocked(!isEvolutionUnlocked);
            // Screen.EvolutionButton.SetInteractable(isEvolutionUnlocked);
            // _uiNotifier.RegisterPin(typeof(IEvolveScreen), Screen.EvolutionButton.Pin);
        }

        private async UniTask InitStateMachine()
        {
            if (_localStateMachine != null)
            {
                _localStateMachine.SetActive(true);
                return;
            }
            
            _localStateMachine = new LocalStateMachine();
            
            UpgradesState menuUpgradesState = new UpgradesState(
                _upgradesScreenProvider, 
                this,
                _logger);
            _localStateMachine.AddState(menuUpgradesState);

            await _localStateMachine.InitializeAsync();
        }

        private void OnUpgradesButtonClicked()
        {
            _localStateMachine.Enter<UpgradesState>();
            _audioService.PlayButtonSound();
        }

        public void OnScreeDispose()
        {
            _logger.Log("upgradesAndEvolutionScreenPresenter DISPOSED", DebugStatus.Warning);
            
            // if (Screen != null)
            // {
            //     Screen.EvolutionStep.CancelStep();
            //     _tutorialManager.UnRegister(Screen.EvolutionStep);
            // }
            
            _localStateMachine?.Cleanup();
            _localStateMachine = null;
            
            _presenter?.Dispose();
            
            Unsubscribe();
        }

        void IDisposable.Dispose()
        {
            OnScreeDispose();
            
            _uiNotifier.UnregisterScreen(this);
        }

        void IGameScreenListener<IUpgradesScreen>.OnScreenOpened(IUpgradesScreen screen) => UpdateState(screen);

        void IGameScreenListener<IUpgradesScreen>.OnInfoChanged(IUpgradesScreen screen) { }

        void IGameScreenListener<IUpgradesScreen>.OnRequiresAttention(IUpgradesScreen screen) => UpdateState(screen);

        void IGameScreenListener<IUpgradesScreen>.OnScreenClosed(IUpgradesScreen screen) { }
        void IGameScreenListener<IUpgradesScreen>.OnScreenActiveChanged(IUpgradesScreen screen, bool isActive) { }
        void IGameScreenListener<IUpgradesScreen>.OnScreenDisposed(IUpgradesScreen screen) { }

        void IGameScreenListener<IEvolveScreen>.OnScreenOpened(IEvolveScreen screen)
        {
            _openEvolveScreenFirst = screen.NeedAttention;
            UpdateState(screen);
        }

        void IGameScreenListener<IEvolveScreen>.OnInfoChanged(IEvolveScreen screen) { }

        void IGameScreenListener<IEvolveScreen>.OnRequiresAttention(IEvolveScreen screen)
        {
            _openEvolveScreenFirst = screen.NeedAttention;
            UpdateState(screen);
        }

        void IGameScreenListener<IEvolveScreen>.OnScreenClosed(IEvolveScreen screen)
        {
            _openEvolveScreenFirst = screen.NeedAttention;
        }
        
        void IGameScreenListener<IEvolveScreen>.OnScreenActiveChanged(IEvolveScreen screen, bool isActive) { }
        void IGameScreenListener<IEvolveScreen>.OnScreenDisposed(IEvolveScreen screen) { }


        private void UpdateState(IGameScreen screen)
        {
            if (_screenStateAggregator.UpdateState(screen))
            {
                RequiresAttention?.Invoke(this);
            }
        }

        // public void UnHighlightUpgradesBtn() => Screen.UpgradesButton.UnHighlight();

        // public void HighlightUpgradesBtn() => Screen.UpgradesButton.Highlight();

        // public void HighlightEvolutionBtn() => Screen.EvolutionButton.Highlight();

        // public void UnHighlightEvolutionBtn() => Screen.EvolutionButton.UnHighlight();

        void IGameScreenListener<IMenuScreen>.OnScreenOpened(IMenuScreen screen) 
        {
            if (NeedAttention)
            {
                RequiresAttention?.Invoke(this);
            }
        }

        void IGameScreenListener<IMenuScreen>.OnInfoChanged(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnRequiresAttention(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnScreenClosed(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnScreenActiveChanged(IMenuScreen screen, bool isActive) { }
        void IGameScreenListener<IMenuScreen>.OnScreenDisposed(IMenuScreen screen) { }
    }
}