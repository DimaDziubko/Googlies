using System;
using _Game.Core._GameInitializer;

namespace _Game.Core._Time
{
    public interface ITimeProvider
    {
        DateTime UtcNow { get; }
    }

    public class TimeProvider : ITimeProvider, IDisposable
    {
        private readonly IGameInitializer _gameInitializer;

        public TimeProvider(
            IGameInitializer gameInitializer
            )
        {
            GlobalTime.Instance = this;

            gameInitializer.OnPreInitialization += Init;
            _gameInitializer = gameInitializer;

            UnbiasedTimeWrapper.Init();
        }

        private void Init()
        {

        }

        void IDisposable.Dispose()
        {
            _gameInitializer.OnPreInitialization -= Init;
        }

        public DateTime UtcNow => GetTime();

        private DateTime GetTime()
        {
            var timeNow = UnbiasedTimeWrapper.GetUnbiasedUtcNow();
            UnityEngine.Debug.Log($"Unbiased time now {timeNow}");
            return timeNow;
        }
    }
}