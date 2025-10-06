using _Game.Core._Logger;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.LiveopsCore.Models.WarriorsFund;
using DevToDev.Analytics;

namespace _Game.LiveopsCore._Trackers
{
    public class WarriorsFundTracker : IGameEventTracker
    {
        private readonly WarriorsFundEvent _event;
        private readonly IUserContainer _userContainer;
        private readonly IMyLogger _logger;
        private readonly IAnalyticsEventSender _sender;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public WarriorsFundTracker(
            WarriorsFundEvent @event,
            IUserContainer userContainer,
            IMyLogger logger,
            IAnalyticsEventSender sender
        )
        {
            _event = @event;
            _userContainer = userContainer;
            _logger = logger;
            _sender = sender;
        }

        void IGameEventTracker.Initialize()
        {
            _event.RoadProgressChanged += TrySentLevelUp;
            _event.Purchased += TrackPurchase;
            _event.AnalyticsCompleted += OnCompleted;
            
           
            TrySendShowed();
            TrySendStart();
            TrySentLevelUp();
        }
        
        void IGameEventTracker.Dispose()
        {
            _event.RoadProgressChanged -= TrySentLevelUp;

            _event.Purchased -= TrackPurchase;
            _event.AnalyticsCompleted -= OnCompleted;
        }
        
        private void TrySendShowed()
        {
            if (!_event.IsOnBreak && !_event.IsLocked && !_event.IsShowSent)
            {
                var parameters = new DTDCustomEventParameters();

                parameters.Add("Document", _event.GetDocumentStr());

                parameters.Add("TimelineID", TimelineState.TimelineNumber);
                parameters.Add("AgeID", TimelineState.AgeNumber);
                parameters.Add("BattleID", TimelineState.BattleNumber);
                parameters.Add("Level", TimelineState.Level);

                _sender.CustomEvent("test_warriorsfund_showed", parameters);
                
                _event.SetShowSent();
            }
            
            _logger.Log("BP SHOWED", DebugStatus.Success);
        }

        private void TrySendStart()
        {
            if (!_event.IsOnBreak && !_event.IsLocked && !_event.IsStartSent)
            {
                var parameters = new DTDCustomEventParameters();

                parameters.Add("Document", _event.GetDocumentStr());

                parameters.Add("TimelineID", TimelineState.TimelineNumber);
                parameters.Add("AgeID", TimelineState.AgeNumber);
                parameters.Add("BattleID", TimelineState.BattleNumber);
                parameters.Add("Level", TimelineState.Level);

                _sender.CustomEvent("test_warriorsfund_started", parameters);
                
                _event.SetStartSent();
            }
            
            _logger.Log("BP STARTED", DebugStatus.Success);
        }

        public void TrackPurchase()
        {
            var parameters = new DTDCustomEventParameters();

            parameters.Add("Document", _event.GetDocumentStr()); //“669 Battle Pass”.

            parameters.Add("TimelineID", TimelineState.TimelineNumber);
            parameters.Add("AgeID", TimelineState.AgeNumber);
            parameters.Add("BattleID", TimelineState.BattleNumber);
            parameters.Add("Level", TimelineState.Level);

            _sender.CustomEvent("test_warriorsfund_purchased", parameters);
        }

        private void OnCompleted()
        {
            var parameters = new DTDCustomEventParameters();

            parameters.Add("Document", _event.GetDocumentStr());

            parameters.Add("TimelineID", TimelineState.TimelineNumber);
            parameters.Add("AgeID", TimelineState.AgeNumber);
            parameters.Add("BattleID", TimelineState.BattleNumber);
            parameters.Add("Level", TimelineState.Level);

            _sender.CustomEvent("test_warriorsfund_completed", parameters);
        }

        private void TrySentLevelUp()
        {
            if ( _event.AnalyticsLevel != 0 && _event.LastLevelSent < _event.AnalyticsLevel)
            {
                var parameters = new DTDCustomEventParameters();

                parameters.Add("Document", _event.GetDocumentStr());
                parameters.Add("WarriorsFundLevel", _event.AnalyticsLevel);

                parameters.Add("TimelineID", TimelineState.TimelineNumber);
                parameters.Add("AgeID", TimelineState.AgeNumber);
                parameters.Add("BattleID", TimelineState.BattleNumber);
                parameters.Add("Level", TimelineState.Level);

                _sender.CustomEvent("test_warriorsfund_level_completed", parameters);
                
                _event.SetLastLevelSent(_event.AnalyticsLevel);
                
                _logger.Log($"test_warriorsfund_level_completed {_event.AnalyticsLevel}", DebugStatus.Info);
            }
        }
    }
}