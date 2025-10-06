using System;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;

namespace _Game.Core.Services._Balancy
{
    public class BalancyProfileChecker : IDisposable
    {
        private readonly IBalancySDKService _balancy;
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;

        private IBattleStatisticsReadonly BattleStatistics => _userContainer.State.BattleStatistics;

        private bool _postInitDone = false;
        private bool _balancyReady = false;
        private bool _initCalled = false;
        
        public BalancyProfileChecker(
            IBalancySDKService balancy,
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IMyLogger logger)
        {
            _logger = logger;
            _gameInitializer = gameInitializer;
            _balancy = balancy;         
            _userContainer = userContainer;

            _gameInitializer.OnPostInitialization += OnPostInit;
            _balancy.Initialized += OnBalancyInit;
        }

        void IDisposable.Dispose()
        {
            _gameInitializer.OnPostInitialization -= Init;
            _balancy.Initialized -= OnBalancyInit;
        }

        private void OnPostInit()
        {
            _postInitDone = true;
            TryInit();
        }

        private void OnBalancyInit()
        {
            _balancyReady = true;
            TryInit();
        }

        private void TryInit()
        {
            if (_initCalled) return;
            if (!_postInitDone || !_balancyReady) return;

            _initCalled = true;
            Init();
        }

        private void Init()
        {
            if (IsResetRequired())
            {
                _balancy.ResetProfile();
                _logger.Log("BALANCY PROFILE CHECKER RESET PROFILE", DebugStatus.Info);
            }
            else
            {
                _balancy.LoadProfile();
                _logger.Log("BALANCY PROFILE CHECKER LOAD PROFILE", DebugStatus.Info);
            }
        }

        private bool IsResetRequired() => 
            BattleStatistics.BattlesCompleted == 0;
    }
}