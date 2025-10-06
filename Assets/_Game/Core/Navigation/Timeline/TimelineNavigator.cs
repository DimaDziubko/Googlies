using System;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay.BattleLauncher;

namespace _Game.Core.Navigation.Timeline
{
    public class TimelineNavigator : ITimelineNavigator, IDisposable
    {
        public event Action TimelineChanged;
        public int CurrentTimelineNumber => _userContainer.State.TimelineState.TimelineNumber;
        public int CurrentTimelineId => _userContainer.State.TimelineState.TimelineId;

        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly ITimelineLoader _timelineLoader;
        private readonly IGameManager _gameManager;
        private readonly IMyLogger _logger;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public TimelineNavigator(
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IMyLogger logger,
            ITimelineLoader timelineLoader,
            IGameManager gameManager)
        {
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _logger = logger;
            _timelineLoader = timelineLoader;
            _gameManager = gameManager;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            TimelineState.NextTimelineOpened += LoadNextTimeline;
            TimelineState.PreviousTimelineOpened += LoadPreviousTimeline;
        }

        void IDisposable.Dispose()
        {
            TimelineState.NextTimelineOpened -= LoadNextTimeline;
            TimelineState.PreviousTimelineOpened -= LoadPreviousTimeline;
            _gameInitializer.OnPostInitialization -= Init;
        }

        public void MoveToNextTimeline() => 
            _userContainer.TimelineStateHandler.OpenNewTimeline();

        public void MoveToPreviousTimeline()
        {
            if(TimelineState.TimelineId > 0)
                _userContainer.TimelineStateHandler.OpenNewTimeline(false);
        }

        private void LoadPreviousTimeline() => 
            _timelineLoader.LoadPreviousTimeline(CurrentTimelineId, OnLoadingCompleted);

        private void LoadNextTimeline() => 
            _timelineLoader.LoadNextTimeline(CurrentTimelineId, OnLoadingCompleted);

        private void OnLoadingCompleted()
        {
            TimelineChanged?.Invoke();
            _gameManager.ChangeTimeline(CurrentTimelineId);
        }
    }
}