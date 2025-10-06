using System;
using ImprovedTimers;
using UnityEngine;

namespace _Game.Utils.Timers
{
    public class SynchronizedCountdownTimer : Timer
    {
        public event Action<float> OnTick;

        private float _tickInterval = 1f;
        private float _timeSinceLastTick;
        
        private float _lastRealTime;

        public SynchronizedCountdownTimer(float value) : base(value)
        {
            _lastRealTime = Time.realtimeSinceStartup;
        }

        public override void Tick()
        {
            if (IsRunning && CurrentTime > 0)
            {
                float realDeltaTime = Time.realtimeSinceStartup - _lastRealTime;
                _lastRealTime = Time.realtimeSinceStartup;
                
                CurrentTime = Mathf.Max(0, CurrentTime - realDeltaTime);
                _timeSinceLastTick += realDeltaTime;
                
                if (_timeSinceLastTick >= _tickInterval)
                {
                    _timeSinceLastTick = 0;
                    OnTick?.Invoke(CurrentTime);
                }
                
                if (CurrentTime <= 0)
                {
                    Stop();
                }
            }
        }

        public override bool IsFinished => CurrentTime <= 0;
    }
}