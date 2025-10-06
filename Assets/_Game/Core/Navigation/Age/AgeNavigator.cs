using System;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay.BattleLauncher;

namespace _Game.Core.Navigation.Age
{
    public class AgeNavigator : IAgeNavigator, IDisposable
    {
        public event Action AgeChanged;
        public int CurrentIdx => _useContainer.State.TimelineState.AgeId;

        private readonly IUserContainer _useContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly ITimelineConfigRepository _config;
        private readonly IMyLogger _logger;
        private readonly IAgeLoader _ageLoader;
        private readonly IGameManager _gameManager;
        private ITimelineStateReadonly TimelineState => _useContainer.State.TimelineState;

        public AgeNavigator(
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IConfigRepository configRepository,
            IAgeLoader ageLoader,
            IMyLogger logger,
            IGameManager gameManager)
        {
            _useContainer = userContainer;
            _gameInitializer = gameInitializer;
            _config = configRepository.TimelineConfigRepository;
            _logger = logger;
            _ageLoader = ageLoader;
            _gameManager = gameManager;
            
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            TimelineState.NextAgeOpened += LoadNextAge;
            TimelineState.PreviousAgeOpened += LoadPreviousAge;
        }
        
        void IDisposable.Dispose()
        {
            TimelineState.NextAgeOpened -= LoadNextAge;
            TimelineState.PreviousAgeOpened -= LoadPreviousAge;
            
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void LoadPreviousAge() => 
            _ageLoader.LoadPreviousAge(TimelineState.TimelineId, TimelineState.AgeId, OnLoadingCompleted);

        private void LoadNextAge() => 
            _ageLoader.LoadNextAge(TimelineState.TimelineId, TimelineState.AgeId, OnLoadingCompleted);

        public bool IsNextAge() => 
            TimelineState.AgeId < _config.LastAgeIdx();

        public void MoveToNextAge()
        {
            if(IsNextAge())
                _useContainer.TimelineStateHandler.OpenNewAge();
        }

        public void MoveToPreviousAge()
        {
            if(IsPreviousAge())
                _useContainer.TimelineStateHandler.OpenNewAge(false);
        }

        public bool IsPreviousAge() => TimelineState.AgeId > 0;
        public bool IsAllBattlesWon() => TimelineState.AllBattlesWon;

        public int GetTotalAgesCount() => _config.AgesCount(TimelineState.TimelineId);

        private void OnLoadingCompleted()
        {
            AgeChanged?.Invoke();
            _gameManager.ChangeAge(CurrentIdx);
            _logger.Log("NEXT AGE LOADED", DebugStatus.Warning);
        }
    }
}