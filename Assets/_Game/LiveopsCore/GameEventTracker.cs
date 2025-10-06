using System;
using System.Collections.Generic;
using _Game.LiveopsCore._Trackers;
using Sirenix.OdinInspector;
using Zenject;

namespace _Game.LiveopsCore
{
    public class GameEventTracker : 
        IInitializable, 
        IDisposable
    {
        private readonly GameEventContainer _container;
        private readonly GameEventStrategyFactory _factory;

        [ShowInInspector]
        private readonly Dictionary<int, IGameEventTracker> _trackers = new();

        public GameEventTracker(
            GameEventContainer container,
            GameEventStrategyFactory factory)
        {
            _container = container;
            _factory = factory;
        }

        void IInitializable.Initialize()
        {
            _container.ActiveEvents.EventAdded += OnEventAdded;
            _container.ActiveEvents.EventRemoved += OnEventRemoved;
        }

        private void OnEventAdded(GameEventBase eventBase)
        {
            if (eventBase.IsOnBreak) return;
            
            if (!_trackers.TryGetValue(eventBase.Id, out var tracker))
            {
                IGameEventTracker eventTracker = _factory.GetTracker(eventBase);
                
                if (eventTracker != null)
                {
                    eventTracker.Initialize();
                    _trackers.Add(eventBase.Id, eventTracker);
                }
            }
        }

        private void OnEventRemoved(GameEventBase eventBase)
        {
            if (eventBase.IsOnBreak) return;
            
            if (_trackers.TryGetValue(eventBase.Id, out var tracker))
            {
                tracker.Dispose();
                _trackers.Remove(eventBase.Id);
            }
        }

        void IDisposable.Dispose()
        {
            _container.ActiveEvents.EventAdded -= OnEventAdded;
            _container.ActiveEvents.EventRemoved -= OnEventRemoved;

            foreach (var tracker in _trackers.Values)
            {
                tracker.Dispose();
            }
            
            _trackers.Clear();
        }
    }
}