using System;
using System.Collections.Generic;
using _Game.Core._GameListenerComposite;
using Zenject;

namespace _Game.LiveopsCore._GameEventCurrencyManagement
{
    public class GameEventCurrencyEarningSystem :
        IStartGameListener,
        IStopGameListener,
        IInitializable,
        IDisposable
    {
        private readonly Dictionary<int, IGameEventCurrencyEarnStrategy> _strategies = new();
        
        private readonly GameEventContainer _container;
        private readonly IGameEventCurrencyEarnStrategyFactory _factory;

        public GameEventCurrencyEarningSystem(
            GameEventContainer container,
            IGameEventCurrencyEarnStrategyFactory factory)
        {
            _container = container;
            _factory = factory;
        }
        
        public void Initialize()
        {
            _container.ActiveEvents.EventAdded += OnEventAdded;
            _container.ActiveEvents.EventRemoved += OnEventRemoved;
            CheckActiveEvents();
        }

        public void Dispose()
        {
            _container.ActiveEvents.EventAdded -= OnEventAdded;
            _container.ActiveEvents.EventRemoved -= OnEventRemoved;
        }

        private void OnEventRemoved(GameEventBase gameEvent)
        {
            if (_strategies.TryGetValue(gameEvent.Id, out IGameEventCurrencyEarnStrategy strategy))
            {
                strategy.UnExecute();
                _strategies.Remove(gameEvent.Id);
            }
        }

        private void OnEventAdded(GameEventBase gameEvent)
        {
            if (_strategies.ContainsKey(gameEvent.Id) || gameEvent.IsOnBreak) return;

            var strategy = _factory.GetEarningStrategy(gameEvent);
            
            if (strategy != null)
            {
                _strategies.Add(gameEvent.Id, strategy);
            }
        }

        private void CheckActiveEvents()
        {
            foreach (var @event in _container.ActiveEvents.GetAll())
            {
                OnEventAdded(@event);
            }
        }

        void IStartGameListener.OnStartBattle()
        {
            foreach (var strategy in _strategies.Values)
            {
                strategy.Execute();
            }
        }

        void IStopGameListener.OnStopBattle()
        {
            foreach (var strategy in _strategies.Values)
            {
                strategy.UnExecute();
            }
        }
    }
}