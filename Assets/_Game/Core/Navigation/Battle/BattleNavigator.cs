using System;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay.BattleLauncher;

namespace _Game.Core.Navigation.Battle
{
    public class BattleNavigator : IBattleNavigator, IDisposable
    {
        public event Action BattleChanged;

        private readonly ITimelineConfigRepository _timelineConfig;
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IAgeNavigator _ageNavigator;
        private readonly ITimelineNavigator _timelineNavigator;
        private readonly IGameManager _gameManager;
        private readonly IMyLogger _logger;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        public bool AllBattlesWon => TimelineState.AllBattlesWon;
        public int LastBattleNumber => _timelineConfig.LastBattle(TimelineState.TimelineId);

        private int _currentBattle;

        public int CurrentBattleIdx
        {
            get => _currentBattle;
            private set
            {
                _currentBattle = value;
                _gameManager.ChangeBattle(_currentBattle);
                BattleChanged?.Invoke();
            }
        }

        public BattleNavigator(
            IConfigRepository configRepository,
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IAgeNavigator ageNavigator,
            ITimelineNavigator timelineNavigator,
            IGameManager gameManager,
            IMyLogger logger)
        {
            _timelineConfig = configRepository.TimelineConfigRepository;
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _ageNavigator = ageNavigator;
            _timelineNavigator = timelineNavigator;
            _logger = logger;
            _gameManager = gameManager;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            _ageNavigator.AgeChanged += OnAgeChanged;
            _timelineNavigator.TimelineChanged += OnTimelineChanged;
            CurrentBattleIdx = TimelineState.MaxBattle;
        }

        void IDisposable.Dispose()
        {
            _ageNavigator.AgeChanged -= OnAgeChanged;
            _timelineNavigator.TimelineChanged -= OnTimelineChanged;
            _gameInitializer.OnPostInitialization -= Init;
        }

        private void OnTimelineChanged() => CurrentBattleIdx = TimelineState.MaxBattle;

        private void OnAgeChanged() => CurrentBattleIdx = TimelineState.MaxBattle;

        public void ForceMoveToNextBattle() => CurrentBattleIdx++;
        public bool IsNextBattle() => !IsLastBattle();

        public void MoveToNextBattle()
        {
            if (CanMoveToNextBattle())
            {
                CurrentBattleIdx++;
            }
        }

        public void MoveToPreviousBattle()
        {
            if (IsPreviousBattle())
            {
                CurrentBattleIdx--;
            }
        }

        public void OpenNextBattle()
        {
            if (CanMoveToNextBattle())
            {
                MoveToNextBattle();
                return;
            }

            if (IsLastBattle()) _userContainer.TimelineStateHandler.SetAllBattlesWon(true);

            if (!IsLastBattle() && !CanMoveToNextBattle())
            {
                _userContainer.TimelineStateHandler.OpenNewBattle(CurrentBattleIdx + 1);
                MoveToNextBattle();
            }
        }

        public bool CanMoveToNextBattle() => CurrentBattleIdx < TimelineState.MaxBattle;
        public void SetAllBattlesWon() => _userContainer.TimelineStateHandler.SetAllBattlesWon(true);
        public void InitLevel() => _userContainer.TimelineStateHandler.InitLevel();
        public bool IsPreviousBattle() => CurrentBattleIdx > 0;
        private bool IsLastBattle() => CurrentBattleIdx == _timelineConfig.LastBattleIndex();
    }
}