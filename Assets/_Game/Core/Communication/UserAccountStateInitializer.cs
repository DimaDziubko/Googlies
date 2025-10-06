using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Services.IAP;
using _Game.Core.UserState;
using _Game.Core.UserState._State;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._UpgradesScreen.Scripts;
using Assets._Game.Core.UserState;

namespace _Game.Core.Communication
{
    public static class UserAccountStateInitializer
    {
        public static CurrenciesState GetInitialCurrencyState()
        {
            return new CurrenciesState
            {
                Cells = DefineCellsData()
            };
        }
        
        public static List<CurrencyCellData> DefineCellsData()
        {
            return new List<CurrencyCellData>
            {
                new() { Type = CurrencyType.Coins, Amount = 0 },
                new() { Type = CurrencyType.Gems, Amount = 1000 },
                new() { Type = CurrencyType.SkillPotion, Amount = 0 },
                new() { Type = CurrencyType.LeaderPassPoint, Amount = 0 }
            };
        }
        public static AdsGemsPackContainer GetInitialAdsGemsPackContainer() => new();

        public static FreeGemsPackContainer GetInitialFreeGemsPackContainer() => new();

        public static CardsCollectionState GetInitialCardsCollectionState()
        {
            return new CardsCollectionState()
            {
                CardSummoningLevel = 1,
                CardsSummoningProgressCount = 0,
                Cards = new List<Card>(),
            };
        }

        public static AdsWeeklyWatchState GetInitialAdsWeeklyWatchState()
        {
            return new AdsWeeklyWatchState()
            {
                LastWeekAdsWatched = new List<int>(7) { 0, 0, 0, 0, 0, 0, 0 },
                LastDay = DateTime.Today
            };
        }

        public static TasksState GetInitialTasksState()
        {
            return new TasksState()
            {
                TotalCompletedTasks = 0
            };
        }

        public static DailyTasksState GetInitialDailyTasksState()
        {
            return new DailyTasksState()
            {
                ProgressOnTask = 0,
                CompletedTasks = new List<int>(),
                CurrentTaskIdx = -1,
                LastTimeGenerated = DateTime.Now
            };
        }

        public static PurchaseDataState GetInitialPurchaseDataState()
        {
            return new PurchaseDataState()
            {
                BoudhtIAPs = new List<BoughtIAP>()
            };
        }

        public static RetentionState GetInitialRetentionState()
        {
            return new RetentionState()
            {
                FirstOpenTime = DateTime.UtcNow,
                FirstDayRetentionEventSent = false,
                SecondDayRetentionEventSent = false,
            };
        }

        public static AdsStatistics GetInitialAdsStatistics()
        {
            return new AdsStatistics()
            {
                AdsReviewed = 0
            };
        }

        public static BattleSpeedState GetInitialBattleSpeedState()
        {
            return new BattleSpeedState()
            {
                IsNormalSpeedActive = true,
                PermanentSpeedId = 0,
                DurationLeft = 0.0f
            };
        }

        public static TutorialState GetInitialTutorialState()
        {
            return new TutorialState()
            {
                CompletedSteps = new List<int>()
                {
                    -1
                }
            };
        }

        public static BattleStatistics GetInitialBattleStatistics()
        {
            return new BattleStatistics()
            {
                BattlesCompleted = 0
            };
        }

        public static FoodBoostState GetInitialFoodBoostState()
        {
            return new FoodBoostState()
            {
                DailyFoodBoostCount = 2,
                LastDailyFoodBoost = DateTime.UtcNow
            };
        }
        
        public static TimelineState GetInitialTimelineState()
        {
            return new TimelineState()
            {
                TimelineId = 0,
                AgeId = 0,
                AllBattlesWon = false,
                MaxBattle = 0,
                OpenUnits = new List<UnitType>(3)
                {
                    UnitType.Light,
                },

                Upgrades = new List<UpgradeItem>()
                {
                    new()
                    {
                        Type = UpgradeItemType.FoodProduction,
                        Level = 0,
                    },

                    new()
                    {
                        Type = UpgradeItemType.BaseHealth,
                        Level = 0,
                    }
                }
            };
        }

        public static List<UnitType> GetInitialOpenUnits()
        {
            return new List<UnitType>()
            {
                UnitType.Light
            };
        }

        public static List<UpgradeItem> GetInitialUpgrades()
        {
            return new List<UpgradeItem>()
            {
                new() { Type = UpgradeItemType.FoodProduction, Level = 0 },
                new() { Type = UpgradeItemType.BaseHealth, Level = 0 },
            };
        }

        public static DungeonsSavegame GetInitialDungeonsSavegame()
        {
            return new DungeonsSavegame
            {
                Dungeons = new List<Dungeon>()
            };
        }

        public static SettingsSaveGame GetInitialSettingsSavegame()
        {
            return new SettingsSaveGame()
            {
                IsDamageTextOn = true,
                IsAmbienceOn = true,
                IsSfxOn = true
            };
        }

        public static SkillCollectionState GetInitialSkillState()
        {
            return new SkillCollectionState()
            {
                ActiveSkills = new List<ActiveSkill>(),
                SkillsCollectedCount = 0
            };
        }
        
        public static AnalyticsStateReadonly GetInitialAnalyticsState()
        {
            return new AnalyticsStateReadonly()
            {
                BoostDifficultCoefficientLastSentDay = DateTime.UtcNow.Date.AddDays(-1),
                ActivityLastSentDay = DateTime.UtcNow.Date.AddDays(-1),
            };
        }
        
        public static EngagementState GetInitialEngagementState()
        {
            return new EngagementState()
            {
                LastMonthLevelCompleted = Enumerable.Repeat(0, 29).ToList(),
                LastDay = DateTime.Today
            };
        }

        public static EventsSavegame GetInitialEventSavegame()
        {
            return new EventsSavegame()
            {
                ActiveEvents = new List<GameEventSavegame>(),
                PastEvents = new List<GameEventSavegame>(),
            };
        }
    }
}