using System;
using _Game.Core._GameInitializer;
using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.Services._Balancy;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;

namespace _Game.Core._EngagementTrackers
{
    public class BalancyProgressTracker : 
        ITimelineChangeListener, 
        IAgeChangeListener, 
        IBattleChangeListener,
        IDisposable
    {
        private readonly IBalancySDKService _balancy;
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        
        private bool _postInitDone = false;
        private bool _balancyReady = false;
        private bool _initCalled = false;
        private bool _balancyProfileLoaded = false;

        public BalancyProgressTracker(
            IBalancySDKService balancy,
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IMyLogger logger)
        {
            _logger = logger;
            _balancy = balancy;
            _userContainer = userContainer;
            
            _gameInitializer = gameInitializer;
            
            _gameInitializer.OnPostInitialization += OnPostInit;
            
            _balancy.Initialized += OnBalancyInit;
            _balancy.ProfileLoaded += OnBalancyProfileLoaded;
        }

        void IDisposable.Dispose()
        {
            _gameInitializer.OnPostInitialization -= OnPostInit;
            _balancy.Initialized -= OnBalancyInit;
            _balancy.ProfileLoaded -= OnBalancyProfileLoaded;
        }

        private void OnBalancyProfileLoaded()
        {
            _balancyProfileLoaded = true;
            TryInit();
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
            if (!_postInitDone || !_balancyReady || !_balancyProfileLoaded) return;

            _initCalled = true;
            Init();
        }

        private void Init()
        {
            TrySendProgress();
        }

        private void TrySendProgress()
        {
            var profile = _balancy.GetProfile();
            if (profile != null)
            {
                profile.Progress.Timeline = TimelineState.TimelineNumber;
                profile.Progress.Age = TimelineState.AgeNumber;
                profile.Progress.MaxBattle = TimelineState.BattleNumber;
            }
        }

        void ITimelineChangeListener.OnTimelineChange(int timeline)
        {
            TrySendProgress();
        }

        void IAgeChangeListener.OnAgeChange(int age)
        {
            TrySendProgress();
        }

        void IBattleChangeListener.OnBattleChange(int battleIndex)
        {
            TrySendProgress();
        }
    }
}