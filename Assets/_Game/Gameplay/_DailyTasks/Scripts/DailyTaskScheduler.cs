using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.DailyTask;
using _Game.Core.Notifications;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._DailyTasks.Scripts.Strategies;
using _Game.UI._Hud;
using Zenject;

namespace _Game.Gameplay._DailyTasks.Scripts
{
    public class DailyTaskScheduler : IInitializable, IDisposable
    {
        private readonly IUserContainer _userContainer;
        private readonly IDailyTaskConfigRepository _dailyTaskConfigRepository;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IMyLogger _logger;

        private readonly BattleHud _battleHud;
        private readonly DailyTaskStrategyFactory _dailyFactory;
        private readonly DailyTaskGenerator _generator;
        private readonly NotificationService _notificationService;
        private IDailyTasksStateReadonly DailyState => _userContainer.State.DailyTasksState;

        private IDailyTaskStrategy _currentStrategy;

        public DailyTaskScheduler(
            IConfigRepository configRepository,
            IUserContainer userContainer,
            IFeatureUnlockSystem featureUnlockSystem,
            IMyLogger logger,
            DailyTaskStrategyFactory dailyFactory,
            DailyTaskGenerator generator,
            BattleHud battleHud,
            NotificationService notificationService)
        {
            _dailyTaskConfigRepository = configRepository.DailyTaskConfigRepository;
            _userContainer = userContainer;
            _featureUnlockSystem = featureUnlockSystem;
            _logger = logger;
            _dailyFactory = dailyFactory;
            _generator = generator;
            _battleHud = battleHud;
            _notificationService = notificationService;
        }

        void IInitializable.Initialize()
        {
            _featureUnlockSystem.FeatureUnlocked += OnFeatureUnlocked;
            if (!_featureUnlockSystem.IsFeatureUnlocked(Feature.DailyTask))
            {
                HideView();
                return;
            }

            TryToStartNewDailyTask();
            SchedulePushNotification();
        }

        private void SchedulePushNotification()
        {
            DateTime? nextGenerationTime = GetNextGenerationTime();

            if (!nextGenerationTime.HasValue)
            {
                _logger.Log("No next generation time calculated. Skipping notification scheduling.",
                    DebugStatus.Warning);
                return;
            }

            DateTime fireTime = nextGenerationTime.Value;

            if (DailyState.ScheduledPushTime == DateTime.MinValue)
            {
                _notificationService.SendDailyTaskAvailableNotification(fireTime);
                _userContainer.DailyTaskStateHandler.ChangeScheduledPushTime(fireTime);
                _logger.Log($"Scheduled new notification for {fireTime} as no previous push was scheduled.",
                    DebugStatus.Info);
            }
            else if (DailyState.ScheduledPushTime < DateTime.UtcNow)
            {
                _notificationService.SendDailyTaskAvailableNotification(fireTime);
                _userContainer.DailyTaskStateHandler.ChangeScheduledPushTime(fireTime);
                _logger.Log($"Scheduled new notification for {fireTime} as the previous scheduled time has passed.",
                    DebugStatus.Info);
            }
            else if (fireTime < DailyState.ScheduledPushTime)
            {
                _notificationService.SendDailyTaskAvailableNotification(fireTime);
                _userContainer.DailyTaskStateHandler.ChangeScheduledPushTime(fireTime);
                _logger.Log($"Scheduled new notification with ID for {fireTime}", DebugStatus.Info);
            }
            else
            {
                _logger.Log("No need to update notification, scheduled time is later.", DebugStatus.Info);
            }
        }

        private void TryToStartNewDailyTask()
        {
            if (DailyTasksRunOut() && !TimeToGenerateNewDailyTasks())
            {
                HideView();
                return;
            }

            if (TimeToGenerateNewDailyTasks())
            {
                _userContainer.DailyTaskStateHandler.ClearCompleted();
                _userContainer.DailyTaskStateHandler.ChangeLastTimeGenerated(DateTime.UtcNow);

                GenerateNewDailyTask();
            }
            else if (NotGeneratedYet())
            {
                GenerateNewDailyTask();
            }
            else if (IsSameDailyTask())
            {
                GenerateNewDailyTask();
            }
            else
            {
                RestoreDailyTask();
            }

            if (DailyState.CompletedTasks.Contains(DailyState.CurrentTaskIdx))
            {
                _logger.Log("SAME DAILY TASK SELECTED", DebugStatus.Fault);
            }
        }

        private bool IsSameDailyTask() =>
            DailyState.CompletedTasks.Contains(DailyState.CurrentTaskIdx);

        private void OnFeatureUnlocked(Feature feature)
        {
            if (feature == Feature.DailyTask)
            {
                TryToStartNewDailyTask();
            }
        }

        private void HideView() =>
            _battleHud.DailyTaskView.Hide();

        private bool DailyTasksRunOut() =>
            DailyState.CompletedTasks.Count >= _dailyTaskConfigRepository.MaxDailyCountPerDay;

        private void RestoreDailyTask()
        {
            DailyTaskModel model = _generator.TryGetPendingDailyTask();
            HandleTask(model);
        }

        private void GenerateNewDailyTask()
        {
            DailyTaskModel model = _generator.GenerateNewRandomTask();
            HandleTask(model);
        }

        private void HandleTask(DailyTaskModel model)
        {
            _currentStrategy = _dailyFactory.GetStrategy(model);
            _currentStrategy.Completed += OnCompleted;
            _currentStrategy.Initialize();
            _currentStrategy.Execute();
        }

        private void OnCompleted()
        {
            _currentStrategy.Completed -= OnCompleted;
            _currentStrategy.Dispose();
            _currentStrategy = null;
            TryToStartNewDailyTask();
        }

        private bool NotGeneratedYet() => DailyState.CurrentTaskIdx == -1;

        private bool TimeToGenerateNewDailyTasks()
        {
            DateTime lastTimeGenerated = DailyState.LastTimeGenerated;
            DateTime now = DateTime.UtcNow;

            if (now.Date > lastTimeGenerated.Date)
            {
                _logger.Log("TIME TO GENERATE DAILY WITH DATE", DebugStatus.Warning);
                return true;
            }

            if (now - lastTimeGenerated >
                TimeSpan.FromMinutes(_dailyTaskConfigRepository.RecoverTimeMinutes))
            {
                _logger.Log("TIME TO GENERATE DAILY WITH TIME", DebugStatus.Warning);
                return true;
            }

            return false;
        }

        private DateTime? GetNextGenerationTime()
        {
            DateTime lastTimeGenerated = DailyState.LastTimeGenerated;
            DateTime now = DateTime.UtcNow;

            DateTime nextMidnight = now.Date.AddDays(1);

            DateTime nextRecoveryTime = lastTimeGenerated.AddMinutes(_dailyTaskConfigRepository.RecoverTimeMinutes);

            DateTime? nextGenerationTime = null;
            if (now < nextMidnight && now < nextRecoveryTime)
            {
                nextGenerationTime = nextMidnight < nextRecoveryTime ? nextMidnight : nextRecoveryTime;
            }
            else if (now < nextMidnight)
            {
                nextGenerationTime = nextMidnight;
            }
            else if (now < nextRecoveryTime)
            {
                nextGenerationTime = nextRecoveryTime;
            }

            return nextGenerationTime;
        }

        void IDisposable.Dispose()
        {
            _featureUnlockSystem.FeatureUnlocked -= OnFeatureUnlocked;

            if (_currentStrategy != null)
            {
                _currentStrategy.Completed -= OnCompleted;
                _currentStrategy.Dispose();
            }
        }
    }
}