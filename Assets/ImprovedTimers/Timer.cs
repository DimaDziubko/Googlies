using System;
using UnityEngine;

namespace ImprovedTimers
{
    public abstract class Timer : IDisposable
    {
        public Action TimerStart = delegate {  };
        public Action TimerStop = delegate {  };
        public float CurrentTime { get; protected set;}
        public bool IsRunning { get; private set;}

        protected float _initialTime;

        public float Progress => Mathf.Clamp(CurrentTime / _initialTime, 0, 1);

        protected Timer(float value)
        {
            _initialTime = value;
        }

        public void Start()
        {
            CurrentTime = _initialTime;
            if (!IsRunning)
            {
                IsRunning = true;
                TimerManager.RegisterTimer(this);
                TimerStart.Invoke();
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                TimerManager.DeregisterTimer(this);
                TimerStop.Invoke();
            }
        }


        public abstract void Tick();
        public abstract bool IsFinished { get; }

        public void Resume() => IsRunning = true;
        public void Pause() => IsRunning = false;

        public virtual void Reset() => CurrentTime = _initialTime;

        public virtual void Reset(float newTime)
        {
            _initialTime = newTime;
            Reset();
        }

        
        
        private bool _disposed;
        
        ~Timer(){Dispose(false);}

        protected virtual void Dispose(bool disposing)
        {
            if(_disposed) return;
            
            if (disposing)
            {
                TimerManager.DeregisterTimer(this);
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}