using System;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Services._Balancy;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Utils._Dtd;
using DevToDev.Analytics;

namespace _Game.Core._EngagementTrackers
{
    public class BalacyEngagementTracker : IDisposable
    {
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IBalancySDKService _balancy;
        private readonly IMyLogger _logger;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private IEngagementState EngagementState => _userContainer.State.EngagementState;
        private IBattleStatisticsReadonly BattleStatistics => _userContainer.State.BattleStatistics;
        private IAnalyticsStateReadonly Analytics => _userContainer.State.AnalyticsStateReadonly;

        private bool _postInitDone = false;
        private bool _balancyReady = false;
        private bool _initCalled = false;
        private bool _balancyProfileLoaded = false;

        public BalacyEngagementTracker(
            IUserContainer userContainer,
            IGameInitializer gameInitializer,
            IBalancySDKService balancy,
            IMyLogger logger)
        {
            _logger = logger;
            _userContainer = userContainer;
            _gameInitializer = gameInitializer;
            _balancy = balancy;
            
            _gameInitializer.OnPostInitialization += OnPostInit;
            _balancy.Initialized += OnBalancyInit;
            _balancy.ProfileLoaded += OnBalancyProfileLoaded;
        }

        private void Init()
        {
            TimelineState.LevelUp += OnLevelUp;
            BattleStatistics.CompletedBattlesCountChanged += OnCompletedBattleChanged;
            EngagementState.ForceEngagementChanged += OnForceEngagementChanged;
            
            SendActivity();
            OnCompletedBattleChanged();
        }
        
        void IDisposable.Dispose()
        {
            _balancy.Initialized -= OnBalancyInit;
            _balancy.ProfileLoaded -= OnBalancyProfileLoaded;
            
            _gameInitializer.OnPreInitialization -= OnPostInit;
            TimelineState.LevelUp -= OnLevelUp;
            BattleStatistics.CompletedBattlesCountChanged -= OnCompletedBattleChanged;
            EngagementState.ForceEngagementChanged -= OnForceEngagementChanged;
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

        private void OnBalancyProfileLoaded()
        {
            _balancyProfileLoaded = true;
            TryInit();
        }

        private void TryInit()
        {
            if (_initCalled) return;
            if (!_postInitDone || !_balancyReady || !_balancyProfileLoaded) return;

            _initCalled = true;
            Init();
        }


        private void OnCompletedBattleChanged()
        {
            var profile =  _balancy.GetProfile();
            if (profile != null)
            {
                profile.Engagement.BattlesCompleted = BattleStatistics.BattlesCompleted;
            }
            
            _logger.Log($"User profile battle completed: {BattleStatistics.BattlesCompleted}", DebugStatus.Info);
        }

        private void OnLevelUp(int obj)
        {
            _userContainer.AnalyticsStateHandler.AddLevelCompleted(DateTime.UtcNow);
            SendActivity();
        }

        private void SendActivity()
        {
            _userContainer.AnalyticsStateHandler.UpdateDate(DateTime.UtcNow);
            
            float activity7 = EngagementState.GetEngagement(7);
            float activity14 = EngagementState.GetEngagement(14);
            float activity21 = EngagementState.GetEngagement(21);
            float activity28 = EngagementState.GetEngagement(28);
            
            var profile =  _balancy.GetProfile();
            if (profile != null)
            {
                profile.Engagement.Activity_7 = activity7;
                profile.Engagement.Activity_14 = activity14;
                profile.Engagement.Activity_21 = activity21;
                profile.Engagement.Activity_28 = activity28;
            }
            
            DateTime currentDate = DateTime.UtcNow;
            
            int daysPassed = (currentDate - Analytics.ActivityLastSentDay).Days;

            if (daysPassed <= 0)
                return;
            
            DTDCustomEventParameters cycleChangedParams = new DTDCustomEventParameters();
            
            cycleChangedParams.Add("Activity 7", activity7);
            cycleChangedParams.Add("Activity 14", activity14);
            cycleChangedParams.Add("Activity 21", activity21);
            cycleChangedParams.Add("Activity 28", activity28);
            
            DTDAnalytics.CustomEvent("player_activity", cycleChangedParams);
            
            _logger.Log($"SetActivity: {cycleChangedParams.ToLogString()}", DebugStatus.Info);
            
            _userContainer.AnalyticsStateHandler.TryChangeActivityLastSentDay(currentDate.Date);
        }

        private void OnForceEngagementChanged()
        {
            float activity7 = EngagementState.GetEngagement(7);
            float activity14 = EngagementState.GetEngagement(14);
            float activity21 = EngagementState.GetEngagement(21);
            float activity28 = EngagementState.GetEngagement(28);
            
            var profile =  _balancy.GetProfile();
            
            if (profile != null)
            {
                profile.Engagement.Activity_7 = activity7;
                profile.Engagement.Activity_14 = activity14;
                profile.Engagement.Activity_21 = activity21;
                profile.Engagement.Activity_28 = activity28;
            }
        }
    }
}