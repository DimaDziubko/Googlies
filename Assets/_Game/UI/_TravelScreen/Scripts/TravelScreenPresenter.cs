using System;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.UI._TimelineInfoScreen.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using _Game.Utils.Disposable;

namespace _Game.UI._TravelScreen.Scripts
{
    public class TravelScreenPresenter :
        ITravelScreenPresenter,
        ITravelScreen,
        IGameScreenEvents<ITravelScreen>,
        IGameScreenListener<IMenuScreen>,
        IDisposable
    {
        public event Action<ITravelScreen> ScreenOpened;
        public event Action<ITravelScreen> InfoChanged;
        public event Action<ITravelScreen> RequiresAttention;
        public event Action<ITravelScreen> ScreenClosed;
        public event Action<ITravelScreen, bool> ActiveChanged;
        public event Action<ITravelScreen> ScreenDisposed;

        public event Action StateChanged;
        public bool IsReviewed { get; private set; }
        public bool NeedAttention => CanTravel;
        public string Info => "Travel";

        private readonly IGameInitializer _gameInitializer;
        private readonly IAgeNavigator _ageNavigator;
        private readonly ITimelineNavigator _timelineNavigator;
        private readonly IBattleNavigator _battleNavigator;
        private readonly IAudioService _audioService;
        private readonly IWorldCameraService _cameraService;
        private readonly ITimelineInfoScreenProvider _infoScreenProvider;
        private readonly IUINotifier _uiNotifier;
        private readonly IMyLogger _logger;

        public TravelScreenPresenter(
            IMyLogger logger,
            IGameInitializer gameInitializer,
            IAgeNavigator ageNavigator,
            ITimelineNavigator timelineNavigator,
            IBattleNavigator battleNavigator,
            IAudioService audioService,
            IWorldCameraService cameraService,
            ITimelineInfoScreenProvider infoScreenProvider,
            IUINotifier uiNotifier)
        {
            _logger = logger;
            _gameInitializer = gameInitializer;
            _ageNavigator = ageNavigator;
            _timelineNavigator = timelineNavigator;
            _battleNavigator = battleNavigator;
            _audioService = audioService;
            _cameraService = cameraService;
            _infoScreenProvider = infoScreenProvider;
            _uiNotifier = uiNotifier;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            _uiNotifier.RegisterScreen(this, this);
            _ageNavigator.AgeChanged += OnStateChanged;
            _timelineNavigator.TimelineChanged += OnStateChanged;
            _battleNavigator.BattleChanged += OnStateChanged;

            IsReviewed = !NeedAttention;

            OnStateChanged();
        }

        void IDisposable.Dispose()
        {
            _uiNotifier.UnregisterScreen(this);
            _ageNavigator.AgeChanged -= OnStateChanged;
            _battleNavigator.BattleChanged -= OnStateChanged;
            _timelineNavigator.TimelineChanged -= OnStateChanged;
            _gameInitializer.OnPostInitialization -= Init;
        }

        void ITravelScreenPresenter.OnScreenOpen()
        {
            IsReviewed = true;
            ScreenOpened?.Invoke(this);
        }

        void ITravelScreenPresenter.OnScreenClosed()
        {
            ScreenClosed?.Invoke(this);
        }

        void ITravelScreenPresenter.OnScreenDisposed()
        {
            _logger.Log("TRAVEL SCREEN DISPOSED", DebugStatus.Info);
            ScreenDisposed?.Invoke(this);
        }

        void ITravelScreenPresenter.OnScreenActiveChanged(bool isActive) =>
            ActiveChanged?.Invoke(this, isActive);

        private void OnStateChanged()
        {
            StateChanged?.Invoke();
            if (NeedAttention)
            {
                IsReviewed = false;
                RequiresAttention?.Invoke(this);
            }
        }

        private bool IsNextTimeline => true;

        public bool CanTravel => _battleNavigator.AllBattlesWon && IsNextTimeline;

        public string GetTravelConditionHint() =>
            $"Win battle {_battleNavigator.LastBattleNumber} first";

        public string GetTravelInfo()
        {
            int nextTimelineNumber = _timelineNavigator.CurrentTimelineNumber + 1;
            return $"Travel to timeline {nextTimelineNumber}";
        }

        private void PlayButtonSound() =>
            _audioService.PlayButtonSound();

        public async void OnTravelButtonClicked()
        {
            PlayButtonSound();

            _timelineNavigator.MoveToNextTimeline();

            TravelAnimationScreenProvider travelAnimationScreenProvider
                = new TravelAnimationScreenProvider(_cameraService.UICameraOverlay);

            Disposable<TimelineInfoScreen> infoScreen = await _infoScreenProvider.Load();

            //var showScreenTask = infoScreen.Value.ShowScreen();

            Disposable<TravelAnimationScreen> animationScreen = await travelAnimationScreenProvider.Load();

            var isAnimationCompleted = await animationScreen.Value.Play();

            if (isAnimationCompleted)
            {
                animationScreen.Dispose();

                infoScreen.Value.PlayFirstAgeAnimation();
            }

            //var isExit = await showScreenTask;
            //if (isExit)
            //{
            //    _infoScreenProvider.Dispose();
            //}
        }

        public string GetTravelText()
        {
            string text = CanTravel
                ? "<color=white>Travel</color>"
                : "<color=red>Travel</color>";
            return text;
        }

        void IGameScreenListener<IMenuScreen>.OnScreenOpened(IMenuScreen screen)
        {
            if (NeedAttention)
            {
                IsReviewed = false;
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