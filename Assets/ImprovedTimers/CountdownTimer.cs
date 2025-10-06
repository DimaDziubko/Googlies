using System;
using UnityEngine;

namespace ImprovedTimers
{
    public class CountdownTimer : Timer
    {
        public event Action<float> OnTick;
        
        private float _tickInterval = 1f;
        private float _timeSinceLastTick;

        public CountdownTimer(float value) : base(value)
        {
        }

        public override void Tick()
        {
            if (IsRunning && CurrentTime > 0)
            {
                CurrentTime -= Time.deltaTime;
                _timeSinceLastTick += Time.deltaTime;

                if (_timeSinceLastTick >= _tickInterval)
                {
                    _timeSinceLastTick = 0;
                    OnTick?.Invoke(CurrentTime);
                }
            }

            if (IsRunning && CurrentTime <= 0)
            {
                Stop();
            }
        }

        public override bool IsFinished => CurrentTime <= 0;
    }
}