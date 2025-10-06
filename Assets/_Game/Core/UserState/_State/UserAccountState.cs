using _Game.Core.Communication;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Core.UserState._State
{
    public class UserAccountState
    {
        public string Version;
        public int Id;
        
        public TimelineState TimelineState;
        public CurrenciesState Currencies;
        public FoodBoostState FoodBoost;
        public BattleStatistics BattleStatistics;
        public TutorialState TutorialState;
        public BattleSpeedState BattleSpeedState;
        public AdsStatistics AdsStatistics;
        public RetentionState RetentionState;
        public PurchaseDataState PurchaseDataState;
        public TasksState TasksState;
        public DailyTasksState DailyTasksState;
        public AdsWeeklyWatchState AdsWeeklyWatchState;
        public CardsCollectionState CardsCollectionState;
        
        public DungeonsSavegame DungeonsSavegame;
        public SkillCollectionState SkillCollectionState;
        
        [ShowInInspector]
        public FreeGemsPackContainer FreeGemsPackContainer;
        
        [ShowInInspector]
        public AdsGemsPackContainer AdsGemsPackContainer;
        
        public SettingsSaveGame SettingsSaveGame;
        public AnalyticsStateReadonly AnalyticsStateReadonly;
        
        public EngagementState EngagementState;
        public EventsSavegame EventsSavegame;

        public object RaceState { get; internal set; }

        public static UserAccountState GetInitial()
        {
            return new UserAccountState()
            {
                Version = Application.version,
                Id = Random.Range(1, int.MaxValue),

                Currencies = UserAccountStateInitializer.GetInitialCurrencyState(),

                TimelineState = UserAccountStateInitializer.GetInitialTimelineState(),
                FoodBoost = UserAccountStateInitializer.GetInitialFoodBoostState(),
                BattleStatistics = UserAccountStateInitializer.GetInitialBattleStatistics(),
                TutorialState = UserAccountStateInitializer.GetInitialTutorialState(),
                BattleSpeedState = UserAccountStateInitializer.GetInitialBattleSpeedState(),
                AdsStatistics = UserAccountStateInitializer.GetInitialAdsStatistics(),
                RetentionState = UserAccountStateInitializer.GetInitialRetentionState(),
                PurchaseDataState = UserAccountStateInitializer.GetInitialPurchaseDataState(),
                DailyTasksState = UserAccountStateInitializer.GetInitialDailyTasksState(),
                TasksState = UserAccountStateInitializer.GetInitialTasksState(),
                AdsWeeklyWatchState = UserAccountStateInitializer.GetInitialAdsWeeklyWatchState(),
                CardsCollectionState = UserAccountStateInitializer.GetInitialCardsCollectionState(),
                FreeGemsPackContainer = UserAccountStateInitializer.GetInitialFreeGemsPackContainer(),
                AdsGemsPackContainer = UserAccountStateInitializer.GetInitialAdsGemsPackContainer(),
                DungeonsSavegame = UserAccountStateInitializer.GetInitialDungeonsSavegame(),
                
                SettingsSaveGame = UserAccountStateInitializer.GetInitialSettingsSavegame(),
                SkillCollectionState = UserAccountStateInitializer.GetInitialSkillState(),
                AnalyticsStateReadonly = UserAccountStateInitializer.GetInitialAnalyticsState(),
                
                EngagementState = UserAccountStateInitializer.GetInitialEngagementState(),
                EventsSavegame = UserAccountStateInitializer.GetInitialEventSavegame(),
            };
        }

        public bool IsValid() => Id > 0;
    }
}