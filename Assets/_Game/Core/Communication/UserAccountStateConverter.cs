using System;
using _Game.Core.UserState._State;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _Game.Core.Communication
{
    public class UserAccountStateConverter : JsonConverter<UserAccountState>
    {
        public override void WriteJson(JsonWriter writer, UserAccountState? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override UserAccountState ReadJson(JsonReader reader, Type objectType, UserAccountState existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            var state = JsonConvert.DeserializeObject<UserAccountState>(JObject.Load(reader).ToString(),
                new JsonSerializerSettings
                {
                    MissingMemberHandling = MissingMemberHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                });

            EnsureDefaultValues(state);

            return state;
        }

        private void EnsureDefaultValues(UserAccountState state)
        {
            state.Currencies ??= UserAccountStateInitializer.GetInitialCurrencyState();
            state.Currencies.Cells ??= new();

            state.TimelineState ??= UserAccountStateInitializer.GetInitialTimelineState();
            state.TimelineState.OpenUnits ??= UserAccountStateInitializer.GetInitialOpenUnits();
            state.TimelineState.Upgrades ??= UserAccountStateInitializer.GetInitialUpgrades();
            state.FoodBoost ??= UserAccountStateInitializer.GetInitialFoodBoostState();
            state.BattleStatistics ??= UserAccountStateInitializer.GetInitialBattleStatistics();
            state.TutorialState ??= UserAccountStateInitializer.GetInitialTutorialState();
            state.BattleSpeedState ??= UserAccountStateInitializer.GetInitialBattleSpeedState();
            state.AdsStatistics ??= UserAccountStateInitializer.GetInitialAdsStatistics();
            state.RetentionState ??= UserAccountStateInitializer.GetInitialRetentionState();
            state.PurchaseDataState ??= UserAccountStateInitializer.GetInitialPurchaseDataState();
            state.DailyTasksState ??= UserAccountStateInitializer.GetInitialDailyTasksState();
            state.TasksState ??= UserAccountStateInitializer.GetInitialTasksState();
            state.AdsWeeklyWatchState ??= UserAccountStateInitializer.GetInitialAdsWeeklyWatchState();
            state.CardsCollectionState ??= UserAccountStateInitializer.GetInitialCardsCollectionState();
            state.FreeGemsPackContainer ??= UserAccountStateInitializer.GetInitialFreeGemsPackContainer();
            state.AdsGemsPackContainer ??= UserAccountStateInitializer.GetInitialAdsGemsPackContainer();
            state.DungeonsSavegame ??= UserAccountStateInitializer.GetInitialDungeonsSavegame();
            state.SkillCollectionState ??= UserAccountStateInitializer.GetInitialSkillState();
            state.SettingsSaveGame ??= UserAccountStateInitializer.GetInitialSettingsSavegame();
            state.AnalyticsStateReadonly ??= UserAccountStateInitializer.GetInitialAnalyticsState();
            state.EngagementState ??= UserAccountStateInitializer.GetInitialEngagementState();
            state.EventsSavegame ??= UserAccountStateInitializer.GetInitialEventSavegame();

            if (state.Currencies != null)
            {
                var requiredCells = UserAccountStateInitializer.DefineCellsData();

                foreach (var cell in requiredCells)
                {
                    if (!state.Currencies.HasCell(cell.Type))
                    {
                        state.Currencies.AddCell(cell);                             
                    }
                }
            }
        }
    }
}