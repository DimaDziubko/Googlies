using _Game.Core._Logger;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.LiveopsCore.Models.BattlePass;
using DevToDev.Analytics;

namespace _Game.LiveopsCore._Trackers
{
    public class BattlePassTracker : IGameEventTracker
    {
        private readonly BattlePassEvent _event;
        private readonly IMyLogger _logger;
        private readonly IAnalyticsEventSender _sender;
        private readonly IUserContainer _userContainer;
        
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public BattlePassTracker(
            BattlePassEvent @event,
            IAnalyticsEventSender sender,
            IUserContainer userContainer,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _sender = sender;
            _event = @event;
            _logger = logger;
        }

        void IGameEventTracker.Initialize()
        {
            _event.Purchased += OnPurchased;
            _event.ObjectiveChanged += OnObjectiveChanged;
            
            _event.AnalyticsCompleted += OnAnalyticsCompleted;
            
            TrySendShowed();
            TrySendStart();
        }

        void IGameEventTracker.Dispose()
        {
            _event.Purchased -= OnPurchased;
            _event.ObjectiveChanged -= OnObjectiveChanged;
            
            _event.AnalyticsCompleted -= OnAnalyticsCompleted;
        }
        
        private void OnAnalyticsCompleted()
        {
            TrySendCompleted();
        }

        private void TrySendShowed()
        {
            if (!_event.IsOnBreak && !_event.IsLocked && !_event.ShowedSent)
            {
                var parameters = new DTDCustomEventParameters();

                parameters.Add("ID", _event.Id);
                parameters.Add("Document", _event.GetDocumentStr());
                parameters.Add("BattlePassDate", _event.GetFormatedTimeForAnalytics());

                parameters.Add("TimelineID", TimelineState.TimelineNumber);
                parameters.Add("AgeID", TimelineState.AgeNumber);
                parameters.Add("BattleID", TimelineState.BattleNumber);
                parameters.Add("Level", TimelineState.Level);

                _sender.CustomEvent("battlepass_showed", parameters);
                
                _event.SetShowedSent();
            }
            
            _logger.Log("BP SHOWED", DebugStatus.Success);
        }

        private void TrySendStart()
        {
            if (!_event.IsOnBreak && !_event.IsLocked && !_event.IsStartSent)
            {
                var parameters = new DTDCustomEventParameters();

                parameters.Add("ID", _event.Id);
                parameters.Add("Document", _event.GetDocumentStr());
                parameters.Add("BattlePassDate", _event.GetFormatedTimeForAnalytics());

                parameters.Add("TimelineID", TimelineState.TimelineNumber);
                parameters.Add("AgeID", TimelineState.AgeNumber);
                parameters.Add("BattleID", TimelineState.BattleNumber);
                parameters.Add("Level", TimelineState.Level);

                _sender.CustomEvent("battlepass_started", parameters);
                
                _event.SetStartSent();
            }
            
            _logger.Log("BP STARTED", DebugStatus.Success);
        }

        private void TrySendCompleted()
        {
            if (!_event.IsOnBreak && !_event.IsLocked)
            {
                var parameters = new DTDCustomEventParameters();

                parameters.Add("ID", _event.Id);
                parameters.Add("Document", _event.GetDocumentStr());
                parameters.Add("BattlePassDate", _event.GetFormatedTimeForAnalytics());

                parameters.Add("TimelineID", TimelineState.TimelineNumber);
                parameters.Add("AgeID", TimelineState.AgeNumber);
                parameters.Add("BattleID", TimelineState.BattleNumber);
                parameters.Add("Level", TimelineState.Level);

                _sender.CustomEvent("battlepass_completed", parameters);
            }
            
            _logger.Log("BP COMPLETED", DebugStatus.Success);
        }

        private void OnPurchased()
        {
            var parameters = new DTDCustomEventParameters();
            parameters.Add("ID", _event.Id); 
            parameters.Add("Document", _event.GetDocumentStr());
            parameters.Add("BattlePassDate", _event.GetFormatedTimeForAnalytics());
            
            parameters.Add("TimelineID", TimelineState.TimelineNumber);
            parameters.Add("AgeID", TimelineState.AgeNumber);
            parameters.Add("BattleID", TimelineState.BattleNumber);
            parameters.Add("Level", TimelineState.Level);
            
            _sender.CustomEvent("battlepass_purchased", parameters);
            
            _logger.Log("BP PURCHASED", DebugStatus.Success);
        }

        private void OnObjectiveChanged()
        {
            var parameters = new DTDCustomEventParameters();

            parameters.Add("ID", _event.Id);
            parameters.Add("Document", _event.GetDocumentStr());
            parameters.Add("BattlePassDate", _event.GetFormatedTimeForAnalytics());
            parameters.Add("BattlePassLevel", _event.NextObjective - 1);

            parameters.Add("TimelineID", TimelineState.TimelineNumber);
            parameters.Add("AgeID", TimelineState.AgeNumber);
            parameters.Add("BattleID", TimelineState.BattleNumber);
            parameters.Add("Level", TimelineState.Level);

            _sender.CustomEvent("battlepass_level_completed", parameters);
        }
    }
}