using System;
using _Game.Core._GameInitializer;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay.BattleLauncher;

namespace _Game.Core._EngagementTrackers
{
    public class LocalLevelTracker : IDisposable
    {
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IGameManager _gameManager;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public LocalLevelTracker(
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IGameManager gameManager)
        {
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _gameManager = gameManager;
            
            _gameInitializer.OnMainInitialization += Init;
        }

        private void Init()
        {
            TimelineState.LevelUp += OnLevelUp;
        }

        private void OnLevelUp(int level)
        {
            _gameManager.ChangeLevel(level);
        }

        void IDisposable.Dispose()
        {
            _gameInitializer.OnMainInitialization -= Init;
        }
    }
}