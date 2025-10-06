using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using DevToDev.Analytics;

namespace _Game.Core.Services.Analytics
{
    public class LevelTracker
    {
        private readonly IUserContainer _userContainer;
        private readonly IMyLogger _logger;
        private readonly IAnalyticsEventSender _sender;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private int TimelineNumber => TimelineState.TimelineId + 1;

        public LevelTracker(
            IUserContainer userContainer,
            IMyLogger logger,
            IAnalyticsEventSender sender)
        {
            _sender = sender;
            _userContainer = userContainer;
            _logger = logger;
        }

        public void Initialize()
        {
            TimelineState.LevelUp += TrackLevel;

        }
        public void Dispose()
        {
            TimelineState.LevelUp -= TrackLevel;
        }

        private void TrackLevel(int level)
        {
            _logger.Log($"TRACK LEVEL {TimelineState.Level}", DebugStatus.Info);
            
            var parametersDTD = new DTDCustomEventParameters();
            parametersDTD.Add("level", TimelineState.Level);
            parametersDTD.Add("TimelineID", TimelineNumber);

            _sender.CustomEvent("level", parametersDTD);
            _sender.LevelUp(TimelineState.Level);
            _sender.SetCurrentLevel(TimelineState.Level);
        }
    }
}