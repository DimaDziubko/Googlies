using _Game.Core._Logger;
using _Game.Core._Time;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Utils;
using System;
using System.Linq;
using UnityEngine;

namespace _Game.Core._EngagementTrackers
{
    public class AFLevelProgressTracker : Zenject.IInitializable, IDisposable
    {
        private const string PP_SAVE_AGE_PROGRESS = "save_age_level_progress";
        private const string PP_SAVE_TIMELINE_PROGRESS = "save_timeline_level_progress";

        private readonly IUserContainer _userContainer;

        private readonly IMyLogger _logger;
        private readonly AppsFlyerAnalyticsService _appsFlyerAnalyticsService;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;


        private string TimelineNumber => (TimelineState.TimelineId + 1).ToString();
        private string AgeNumber => (TimelineState.AgeId + 1).ToString();
        private string BattleNumber => (TimelineState.MaxBattle).ToString();


        private LevelProgressSaveData _ageLevelProgressSave;
        private LevelProgressSaveData _timelineLevelProgressSave;


        // Add the levels you want to track
        private readonly int[] _agesToTrack = new int[] { 2, 3, 4, 5, 6 };
        private readonly int[] _timelinesToTrack = new int[] { 2, 3, 5, 7, 10 };

        public AFLevelProgressTracker(
            IUserContainer userContainer,
            IMyLogger logger,
            AppsFlyerAnalyticsService appsFlyerAnalyticsService
            )
        {
            _userContainer = userContainer;
            _logger = logger;
            _appsFlyerAnalyticsService = appsFlyerAnalyticsService;
        }

        public void Initialize()
        {
            Init();
        }

        public void Dispose()
        {
            TimelineState.NextTimelineOpened -= OnNextTimelineOpened;

            TimelineState.NextAgeOpened -= OnNextAgeOpened;
        }

        private void Init()
        {
            _ageLevelProgressSave = LoadLevelProgressSave(PP_SAVE_AGE_PROGRESS);
            _timelineLevelProgressSave = LoadLevelProgressSave(PP_SAVE_TIMELINE_PROGRESS);

            TimelineState.NextTimelineOpened += OnNextTimelineOpened;

            TimelineState.NextAgeOpened += OnNextAgeOpened;

            _logger.Log("LevelProgressTracker init Subscribed");
        }

        private void OnNextAgeOpened()
        {
            var localAgeNumber = TimelineState.AgeId + 1;
            OnAgeLevelComplete();
            OnAgeLevelStart(localAgeNumber);
        }

        private void OnNextTimelineOpened()
        {
            var timelineNumber = TimelineState.TimelineId + 1;
            OnTimelineLevelComplete();
            OnTimelineLevelStart(timelineNumber);
        }

        public void OnAgeLevelStart(int levelNumber)
        {
            if (TimelineState.TimelineId >= 1) return;
            var currentLevel = levelNumber;

            if (_ageLevelProgressSave.Level == currentLevel) return;

            // Check if the current level is in the list of levels to track
            if (!_agesToTrack.Contains(currentLevel)) return;

            DateTime currentTime = DateTime.UtcNow;

            _logger.Log($"LevelProgressTracker Age {currentLevel} started at {currentTime}");

            _ageLevelProgressSave = new LevelProgressSaveData();
            _ageLevelProgressSave.Level = levelNumber;
            _ageLevelProgressSave.TimeStart = currentTime.ToString("O");

            SaveLevelProgress(PP_SAVE_AGE_PROGRESS, _ageLevelProgressSave);
        }

        public void OnAgeLevelComplete()
        {
            // Check if the level completed is one we are tracking
            if (!_agesToTrack.Contains(_ageLevelProgressSave.Level)) return;

            DateTime currentTime = DateTime.UtcNow;

            var levelStartTime = DateTime.Parse(_ageLevelProgressSave.TimeStart);
            TimeSpan timeTaken = currentTime - levelStartTime;


            // Send event to AppsFlyer
            _appsFlyerAnalyticsService.SendAgeLevelCompleteEvent(_ageLevelProgressSave.Level, timeTaken);
            _logger.Log($"LevelProgressTracker Age TimeNow {currentTime} started at {levelStartTime}");

            _ageLevelProgressSave = new LevelProgressSaveData();
            SaveLevelProgress(PP_SAVE_AGE_PROGRESS, _ageLevelProgressSave);

            // Send event to devtodev
        }

        public void OnTimelineLevelStart(int timelineNumber)
        {
            if (_timelineLevelProgressSave.Level == timelineNumber) return;
            if (!_timelinesToTrack.Contains(timelineNumber)) return;

            DateTime currentTime = GlobalTime.UtcNow;

            _logger.Log($"LevelProgressTracker Timeline Level {timelineNumber} started at {currentTime}");

            _timelineLevelProgressSave = new LevelProgressSaveData
            {
                Level = timelineNumber,
                TimeStart = currentTime.ToString("O")
            };

            SaveLevelProgress(PP_SAVE_TIMELINE_PROGRESS, _timelineLevelProgressSave);
        }

        public void OnTimelineLevelComplete()
        {
            if (!_timelinesToTrack.Contains(_timelineLevelProgressSave.Level)) return;

            DateTime currentTime = GlobalTime.UtcNow;

            var levelStartTime = DateTime.Parse(_timelineLevelProgressSave.TimeStart);
            TimeSpan timeTaken = currentTime - levelStartTime;

            _appsFlyerAnalyticsService.SendTimeLineLevelCompleteEvent(_timelineLevelProgressSave.Level, timeTaken);
            _logger.Log($"LevelProgressTracker Timeline TimeNow {currentTime} started at {levelStartTime}");

            _timelineLevelProgressSave = new LevelProgressSaveData();
            SaveLevelProgress(PP_SAVE_TIMELINE_PROGRESS, _timelineLevelProgressSave);
        }

        private LevelProgressSaveData LoadLevelProgressSave(string saveStr)
        {
            string json = PlayerPrefs.GetString(saveStr);

            var progress = JSONUtils.ConvertToObj<LevelProgressSaveData>(json) ?? new LevelProgressSaveData();

            return progress;
        }

        private void SaveLevelProgress(string saveStr, LevelProgressSaveData saveData)
        {
            PlayerPrefs.SetString(saveStr, JSONUtils.ConvertToString(saveData));
        }

        public class LevelProgressSaveData
        {
            public int Level;
            public string TimeStart;
        }
    }
}
