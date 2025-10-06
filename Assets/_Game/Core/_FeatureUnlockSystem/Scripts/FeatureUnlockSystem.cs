using System;
using System.Collections.Generic;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Utils;
using Assets._Game.Core.UserState;

namespace _Game.Core._FeatureUnlockSystem.Scripts
{
    public class FeatureUnlockSystem : IFeatureUnlockSystem, IDisposable, ISaveGameTrigger
    {
        public event Action<bool> SaveGameRequested;
        public event Action<Feature> FeatureUnlocked;

        private const int BATTLE_SPEED_AGE_THRESHOLD = 1;
        private const int PAUSE_AGE_THRESHOLD = 1;

        private readonly IUserContainer _persistentData;
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;

        private readonly Dictionary<Feature, bool> _featureUnlockState = new();
        private ITutorialStateReadonly TutorialState => _persistentData.State.TutorialState;
        private ITimelineStateReadonly TimelineState => _persistentData.State.TimelineState;
        private IBattleStatisticsReadonly BattleStatisticsState => _persistentData.State.BattleStatistics;

        private readonly IFeatureConfigRepository _config;

        public FeatureUnlockSystem(
            IUserContainer persistentData,
            IGameInitializer gameInitializer,
            IConfigRepository configRepository,
            IMyLogger logger)
        {
            _persistentData = persistentData;
            _gameInitializer = gameInitializer;
            _logger = logger;
            _config = configRepository.FeatureConfigRepository;
            gameInitializer.OnPostInitialization += Init;
        }

        private void Init()
        {
            foreach (Feature feature in Enum.GetValues(typeof(Feature)))
            {
                _featureUnlockState[feature] = CheckInitialUnlockState(feature);
            }

            TutorialState.StepsCompletedChanged += OnTutorialStepCompleted;
            BattleStatisticsState.CompletedBattlesCountChanged += OnBattleStatisticsChanged;
            TimelineState.NextAgeOpened += OnNextAgeOpened;
            TimelineState.NextTimelineOpened += OnNextTimelineOpened;
        }

        void IDisposable.Dispose()
        {
            TimelineState.NextAgeOpened -= OnNextAgeOpened;
            TimelineState.NextTimelineOpened -= OnNextTimelineOpened;
            TutorialState.StepsCompletedChanged -= OnTutorialStepCompleted;
            BattleStatisticsState.CompletedBattlesCountChanged -= OnBattleStatisticsChanged;
            _gameInitializer.OnPostInitialization -= Init;
        }

        public bool IsFeatureUnlocked(Feature feature)
        {
            if (_featureUnlockState.TryGetValue(feature, out bool isUnlocked))
            {
                //_logger.Log($"Ask for feature {feature}, isUnlocked {isUnlocked}", DebugStatus.Warning);
                return isUnlocked;
            }

            return false;
        }

        private bool CheckInitialUnlockState(Feature feature)
        {
            return feature switch
            {
                Feature.None => true,
                Feature.Pause => GetThresholdFor(GetThresholdForPause),
                Feature.FoodBoost => GetThresholdFor(GetThresholdForFoodBoost),
                Feature.AlwaysUnlocked => true,
                Feature.UpgradesScreen => GetThresholdFor(GetThresholdForUpgradesScreen),
                Feature.EvolutionScreen => GetThresholdFor(GetThresholdForEvolutionScreen),
                Feature.BattleSpeed => GetThresholdFor(GetThresholdForBattleSpeed),
                Feature.X2 => GetThresholdFor(GetThresholdForX2),
                Feature.Shop => GetThresholdFor(GetThresholdForShop),
                Feature.DailyTask => GetThresholdFor(GetThresholdForDailyTask),
                Feature.Cards => GetThresholdFor(GetThresholdForCards),
                Feature.GemsShopping => GetThresholdFor(GetThresholdForGemsShopping),
                Feature.Dugneons => GetThresholdFor(GetThresholdForDungeons),
                Feature.Skills => GetThresholdFor(GetThresholdForSkills),
                _ => false
            };
        }
        
        private bool GetThresholdForSkills()
        {
            if (_config != null)
            {
                return TimelineState.TimelineNumber >= _config.SkillRequiredTimeline
                       && _config.IsSkillsUnlocked;
            }
            
            return false;
        }

        private bool GetThresholdForDungeons()
        {
            if (_config != null)
            {
                return TimelineState.TimelineId >= Constants.FeatureThresholds.DUNGEONS_TIMELINE_THRESHOLD &&
                       _config.IsDungeonsUnlocked;
            }

            return false;
        }

        private bool GetThresholdForGemsShopping() => 
            TutorialState.CompletedSteps.Contains(Constants.TutorialSteps.CARDS_PURCHASE);

        private bool GetThresholdFor(Func<bool> thresholdFunc) => thresholdFunc();

        private bool GetThresholdForCards() => GetThresholdForShop();

        private bool GetThresholdForDailyTask() =>
            /* TutorialState.CompletedSteps.Contains(Constants.TutorialSteps.EVOLVE) && */
            BattleStatisticsState.BattlesCompleted >= Constants.FeatureThresholds.DAILY_BATTLE_THRESHOLD;

        private bool GetThresholdForShop() => 
            /* TutorialState.CompletedSteps.Contains(Constants.TutorialSteps.EVOLVE) && */
            BattleStatisticsState.BattlesCompleted >= Constants.FeatureThresholds.SHOP_BATTLE_THRESHOLD;

        private bool GetThresholdForX2() =>
            /* TutorialState.CompletedSteps.Contains(Constants.TutorialSteps.FOOD_UPGRADE) && */
            BattleStatisticsState.BattlesCompleted >= Constants.FeatureThresholds.X2_BATTLE_THRESHOLD;

        private bool GetThresholdForBattleSpeed() => false;

        //TimelineState.TimelineId > 0 || TimelineState.AgeId >= BATTLE_SPEED_AGE_THRESHOLD;

        private bool GetThresholdForEvolutionScreen() =>
            /* TutorialState.CompletedSteps.Contains(Constants.TutorialSteps.FOOD_UPGRADE) && */
            BattleStatisticsState.BattlesCompleted >= Constants.FeatureThresholds.EVOLUTION_SCREEN_BATTLE_THRESHOLD;

        private bool GetThresholdForFoodBoost() =>
            /* TutorialState.CompletedSteps.Contains(Constants.TutorialSteps.FOOD_UPGRADE) && */
            BattleStatisticsState.BattlesCompleted >= Constants.FeatureThresholds.FOOD_BOOST_BATTLE_THRESHOLD;

        private bool GetThresholdForPause() => 
            TimelineState.AgeId >= PAUSE_AGE_THRESHOLD || TimelineState.TimelineId > 0;

        private bool GetThresholdForUpgradesScreen() =>
            /* TutorialState.CompletedSteps.Contains(Constants.TutorialSteps.BUILDER) && */
            BattleStatisticsState.BattlesCompleted >= Constants.FeatureThresholds.UPGRADES_SCREEN_BATTLE_THRESHOLD;

        private void OnBattleStatisticsChanged() => CheckForFeaturesUnlock();
        private void OnTutorialStepCompleted(int step) => CheckForFeaturesUnlock();
        private void OnNextTimelineOpened() => CheckForFeaturesUnlock();
        private void OnNextAgeOpened() => CheckForFeaturesUnlock();

        private void CheckForFeaturesUnlock()
        {
            CheckAndUnlockFeature(Feature.Cards, GetThresholdForCards);
            CheckAndUnlockFeature(Feature.DailyTask, GetThresholdForDailyTask);
            CheckAndUnlockFeature(Feature.BattleSpeed, GetThresholdForBattleSpeed);
            CheckAndUnlockFeature(Feature.EvolutionScreen, GetThresholdForEvolutionScreen);
            CheckAndUnlockFeature(Feature.Shop, GetThresholdForShop);
            CheckAndUnlockFeature(Feature.UpgradesScreen, GetThresholdForUpgradesScreen);
            CheckAndUnlockFeature(Feature.FoodBoost, GetThresholdForFoodBoost);
            CheckAndUnlockFeature(Feature.X2, GetThresholdForX2);
            CheckAndUnlockFeature(Feature.Pause, GetThresholdForPause);
            CheckAndUnlockFeature(Feature.GemsShopping, GetThresholdForGemsShopping);
            CheckAndUnlockFeature(Feature.Dugneons, GetThresholdForDungeons);
            CheckAndUnlockFeature(Feature.Skills, GetThresholdForSkills);
        }

        private void CheckAndUnlockFeature(Feature feature, Func<bool> getThresholdFunc)
        {
            if (!IsFeatureUnlocked(feature) && getThresholdFunc())
            {
                _featureUnlockState[feature] = true;
                FeatureUnlocked?.Invoke(feature);
                SaveGameRequested?.Invoke(false);
            }
        }

        public bool IsFeatureUnlocked(IFeature feature) => IsFeatureUnlocked(feature.Feature);
    }
}