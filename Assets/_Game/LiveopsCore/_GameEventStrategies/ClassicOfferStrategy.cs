using System;
using _Game.Core._Logger;
using _Game.Core.Services.IAP._Processors;
using _Game.LiveopsCore;
using _Game.LiveopsCore._GameEventStrategies;
using _Game.LiveopsCore.Models.ClassicOffer;
using _Game.Utils.Timers;

namespace _Game.Core._GameEventInfrastructure._GameEventStrategies
{
    public class ClassicOfferStrategy : ICycleGameEventStrategy
    {
        public event Action<GameEventBase> Complete;
        public event Action<int> CycleChanged;

        private readonly ClassicOfferEvent _event;
        private readonly IMyLogger _logger;
        private readonly ClassicOfferPurchaseProcessor _purchaseProcessor;

        private SynchronizedCountdownTimer _eventTimer;

        public ClassicOfferStrategy(
            ClassicOfferEvent @event,
            IMyLogger logger,
            ClassicOfferPurchaseProcessor purchaseProcessor
        )
        {
            _event = @event;
            _logger = logger;
            _purchaseProcessor = purchaseProcessor;
        }

        public void Execute()
        {
            InitEventTimer();

            _purchaseProcessor.Initialize();

            if (!_event.IsOnBreak)
            {
                _event.NotifyShowed();
            }
            _event.OnOfferPurchased += OfferPurchased;


            if (IsEventExpired()) OnEventTimerStop();

        }

        private void OfferPurchased()
        {
            if (_event.GetAvalivableCount <= 0)
            {
                _event.Save.SetHiden(true);
                _event.RequestHideEvent();
            }
        }

        private void InitEventTimer()
        {
            if (_eventTimer == null)
                _eventTimer = new SynchronizedCountdownTimer(_event.EventTimeLeft);
            else
                _eventTimer.Reset(_event.EventTimeLeft);

            _eventTimer.TimerStop += OnEventTimerStop;
            _eventTimer.OnTick += OnEventTimerTick;

            _eventTimer.Start();
        }

        private void ChangeCycleByTimer()
        {
            ChangeCycle();
            _eventTimer.Reset(_event.EventTimeLeft);
            _eventTimer.Start();
        }

        public void UnExecute()
        {

            //Complete?.Invoke(_event);
            _event.Save.SetHiden(false);
            _eventTimer.Pause();
            _event.NotifyCompleted();
        }

        public void Cleanup()
        {
            _purchaseProcessor.Dispose();
             _event.OnOfferPurchased -= OfferPurchased;

            if (_eventTimer != null)
            {
                _eventTimer.TimerStop -= OnEventTimerStop;
                _eventTimer.OnTick -= OnEventTimerTick;
                _eventTimer.Dispose();
            }
        }

        private bool CycleExpired() =>
            _event.EventTimeLeft <= 0;

        private void ChangeCycle()
        {
            _event.Save.SetHiden(false);
            Complete?.Invoke(_event);
        }

        private bool IsEventExpired()
        {
            _logger.Log($"Classic Offer EXPIRED {_event.EventTimeLeft <= 0}", DebugStatus.Info);
            return _event.EventTimeLeft <= 0;
        }

        private void OnEventTimerTick(float timeLeft)
        {
            _event.InvokeEventTimerTick(timeLeft);
        }

        private void OnEventTimerStop()
        {
            _logger.Log($"BATTLE PASS ON TIMER STOP", DebugStatus.Info);
            _event.NotifyCompleted();
            ChangeCycleByTimer();
        }

    }
}
