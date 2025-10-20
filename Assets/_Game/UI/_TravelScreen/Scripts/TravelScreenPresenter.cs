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
        IDisposable
    {

        public event Action StateChanged;
        public bool IsReviewed { get; private set; }
        public bool NeedAttention => CanTravel;
        public string Info => "Travel";

        private readonly IGameInitializer _gameInitializer;
        private readonly IAgeNavigator _ageNavigator;
        private readonly ITimelineNavigator _timelineNavigator;
        private readonly IBattleNavigator _battleNavigator;
        private readonly IAudioService _audioService;
        private readonly IUINotifier _uiNotifier;
        private readonly IMyLogger _logger;

        public TravelScreenPresenter(
            IMyLogger logger,
            IGameInitializer gameInitializer,
            IAgeNavigator ageNavigator,
            ITimelineNavigator timelineNavigator,
            IBattleNavigator battleNavigator,
            IAudioService audioService
            )
        {
            _logger = logger;
            _gameInitializer = gameInitializer;
            _ageNavigator = ageNavigator;
            _timelineNavigator = timelineNavigator;
            _battleNavigator = battleNavigator;
            _audioService = audioService;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            _ageNavigator.AgeChanged += OnStateChanged;
            _timelineNavigator.TimelineChanged += OnStateChanged;
            _battleNavigator.BattleChanged += OnStateChanged;

            IsReviewed = !NeedAttention;

            OnStateChanged();
        }

        void IDisposable.Dispose()
        {
            _ageNavigator.AgeChanged -= OnStateChanged;
            _battleNavigator.BattleChanged -= OnStateChanged;
            _timelineNavigator.TimelineChanged -= OnStateChanged;
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void OnStateChanged()
        {
            StateChanged?.Invoke();
            if (NeedAttention)
            {
                IsReviewed = false;
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

            //Disposable<TravelAnimationScreen> animationScreen = await travelAnimationScreenProvider.Load();

            //var isAnimationCompleted = await animationScreen.Value.Play();

            //if (isAnimationCompleted)
            //{
            //    animationScreen.Dispose();

            //    infoScreen.Value.PlayFirstAgeAnimation();
            //}
        }

        public string GetTravelText()
        {
            string text = CanTravel
                ? "<color=white>Travel</color>"
                : "<color=red>Travel</color>";
            return text;
        }
    }
}