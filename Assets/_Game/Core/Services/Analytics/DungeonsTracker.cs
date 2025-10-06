using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core.Boosts;
using _Game.Core.Configs.Repositories;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Utils;
using DevToDev.Analytics;

namespace _Game.Core.Services.Analytics
{
    public class DungeonsTracker
    {
        private readonly IUserContainer _userContainer;
        private readonly IDungeonModelFactory _factory;
        private readonly IFeatureConfigRepository _config;
        private readonly IAnalyticsEventSender _sender;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private IDungeonsStateReadonly DungeonsState => _userContainer.State.DungeonsSavegame;
        
        public DungeonsTracker(
            IUserContainer userContainer,
            IDungeonModelFactory factory,
            IFeatureConfigRepository config,
            IAnalyticsEventSender sender)
        {
            _sender = sender;
            _userContainer = userContainer;
            _factory = factory;
            _config = config;
        }


        public void Initialize()
        {
            TimelineState.NextTimelineOpened += OnTimelineOpened;
            foreach (var model in _factory.GetModels())
            {
                model.LevelUpWithInfo += OnLevelUp;
            }

            foreach (var dungeon in DungeonsState.Dungeons)
            {
                dungeon.KeysAdded += OnKeyAdded;
            }
        }
        
        private void OnKeyAdded(int amount, DungeonType type, ItemSource source)
        {
            var model = _factory.GetModel(type);

            int maxAvailableLevel = model.MaxAvailableLevel;
            int maxAvailableStage = model.MaxAvailableStage;
            int maxAvailableSubLevel = model.MaxAvailableSubLevel;

            var levelKey = $"{type}_Level";
            var stageKey = $"{type}_Timeline";
            var subLevelKey = $"{type}_Battle";


            var dtdKeysEventParameters = new DTDCustomEventParameters();

            dtdKeysEventParameters.Add("KeysType", type.ToString());
            dtdKeysEventParameters.Add("Source", source.ToString());
            dtdKeysEventParameters.Add("Amount", amount);

            dtdKeysEventParameters.Add("Level", TimelineState.Level);
            dtdKeysEventParameters.Add("TimelineID", TimelineState.TimelineNumber);
            dtdKeysEventParameters.Add("AgeID", TimelineState.AgeNumber);
            dtdKeysEventParameters.Add("BattleID", (TimelineState.BattleNumber));

            dtdKeysEventParameters.Add(levelKey, maxAvailableLevel);
            dtdKeysEventParameters.Add(stageKey, maxAvailableStage);
            dtdKeysEventParameters.Add(subLevelKey, maxAvailableSubLevel);

            _sender.CustomEvent("keys_get", dtdKeysEventParameters);
        }

        public void Dispose()
        {
            TimelineState.NextTimelineOpened -= OnTimelineOpened;
            
            foreach (var model in _factory.GetModels())
            {
                model.LevelUpWithInfo -= OnLevelUp;
            }
        }

        private void OnLevelUp(DungeonType type, int maxAvailableLevel, int maxAvailableStage, int maxAvailableSubLevel)
        {
            var levelKey = $"{type}_Level";
            var stageKey = $"{type}_Timeline";
            var subLevelKey = $"{type}_Battle";

            _sender.SetUserData(levelKey, maxAvailableLevel);
            _sender.SetUserData(stageKey, maxAvailableStage);
            _sender.SetUserData(subLevelKey, maxAvailableSubLevel);
        }

        private void OnTimelineOpened()
        {
            if (TimelineState.TimelineId == Constants.FeatureThresholds.DUNGEONS_TIMELINE_THRESHOLD)
            {
                foreach (var model in _factory.GetModels())
                {
                    OnLevelUp(model.DungeonType, model.MaxAvailableLevel, model.MaxAvailableStage, model.MaxAvailableSubLevel);
                }

                //DTD
                var dungeonOpenedParameters = new DTDCustomEventParameters();
                dungeonOpenedParameters.Add("TimelineID", TimelineState.TimelineNumber);
                dungeonOpenedParameters.Add("Level", TimelineState.Level);
                _sender.CustomEvent("dungeon_opened", dungeonOpenedParameters);


                var dungeonTimeline2OpenedParameters = new DTDCustomEventParameters();
                dungeonTimeline2OpenedParameters.Add("TimelineID", TimelineState.TimelineNumber);
                dungeonTimeline2OpenedParameters.Add("Level", TimelineState.Level);
                dungeonTimeline2OpenedParameters.Add("IsOpened", _config.IsDungeonsUnlocked);
                _sender.CustomEvent("dungeon_timeline2_opened", dungeonTimeline2OpenedParameters);
            }
        }
    }
}