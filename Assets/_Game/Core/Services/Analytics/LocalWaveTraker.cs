using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using DevToDev.Analytics;
using System;
using System.Linq;

namespace _Game.Core.Services.Analytics
{
    public class LocalWaveTraker : IDisposable
    {
        private readonly IUserContainer _userContainer;
        private readonly IMyLogger _logger;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        private int TimelineNumber => TimelineState.TimelineId + 1;
        private int AgeNumber => TimelineState.AgeId + 1;
        private int BattleNumberWithoutZero => TimelineState.BattleNumber;

        public LocalWaveTraker(
            IMyLogger logger,
            IUserContainer userContainer
            )
        {
            _userContainer = userContainer;
            _logger = logger;

        }

        public void Initialize()
        {
            TimelineState.OnTrackWave += OnTrackWaveEvent;
        }

        public void Dispose()
        {
            TimelineState.OnTrackWave -= OnTrackWaveEvent;
        }

        public void TrackWaveEvent(int wave)
        {
            TimelineState.TrackWave(wave);
        }

        private void OnTrackWaveEvent(int currentWave)
        {
            var parameters = new DTDCustomEventParameters();

            parameters.Add("WaveID", currentWave);

            parameters.Add("BattleID", BattleNumberWithoutZero);
            parameters.Add("AgeID", AgeNumber);
            parameters.Add("TimelineID", TimelineNumber);
            parameters.Add("Level", TimelineState.Level);

            SendEvent("wave_started", parameters);
        }

        private void SendEvent(string eventName, DTDCustomEventParameters parameters)
        {
            DTDAnalytics.CustomEvent(eventName, parameters);
            _logger.Log($"Dev2Dev Event With Params {eventName}");

            string parametersLog = string.Join(", ", parameters.GetAllParameters().Select(kvp => $"{kvp.Key}: {kvp.Value}"));
            _logger.Log($"Dev2Dev| Params: {parametersLog}");
        }
    }
}
