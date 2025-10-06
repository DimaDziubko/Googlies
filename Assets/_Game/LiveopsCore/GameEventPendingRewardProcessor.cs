using System;
using System.Collections.Generic;
using _Game.Core.Services.UserContainer;
using _Game.LiveopsCore._GrantStrategies;
using Zenject;

namespace _Game.LiveopsCore
{
    public class GameEventPendingRewardProcessor :
        IInitializable,
        IDisposable,
        ISaveGameTrigger
    {
        public event Action<bool> SaveGameRequested;
        
        private readonly GameEventContainer _container;
        private readonly GameEventStrategyFactory _factory;

        private readonly Dictionary<int, IGameEventUnclaimedRewardGrantStrategy> _pastEventStrategies = new();
        private readonly Dictionary<int, IGameEventUnclaimedRewardGrantStrategy> _cycledEventStrategies = new();

        public GameEventPendingRewardProcessor(
            GameEventContainer container,
            GameEventStrategyFactory factory)
        {
            _container = container;
            _factory = factory;
        }

        void IInitializable.Initialize()
        {
            _container.PastEventsWithPendingRewards.EventAdded += OnPastEventAdded;
            _container.PastEventsWithPendingRewards.EventRemoved += OnPastEventRemoved;

            _container.CycledEventsWithPendingRewards.EventAdded += OnCycledEventAdded;
            _container.CycledEventsWithPendingRewards.EventRemoved += OnCycledEventRemoved;

            Process();
        }

        void IDisposable.Dispose()
        {
            _container.PastEventsWithPendingRewards.EventAdded -= OnPastEventAdded;
            _container.PastEventsWithPendingRewards.EventRemoved -= OnPastEventRemoved;

            _container.CycledEventsWithPendingRewards.EventAdded -= OnCycledEventAdded;
            _container.CycledEventsWithPendingRewards.EventRemoved -= OnCycledEventRemoved;

            UnsubscribeAndCleanup(_pastEventStrategies);
            UnsubscribeAndCleanup(_cycledEventStrategies);
        }

        private void Process()
        {
            foreach (var ev in _container.PastEventsWithPendingRewards.GetAll())
                HandlePast(ev);

            foreach (var ev in _container.CycledEventsWithPendingRewards.GetAll())
                HandleCycled(ev);
        }

        private void OnPastEventAdded(GameEventBase ev) => HandlePast(ev);
        private void OnCycledEventAdded(GameEventBase ev) => HandleCycled(ev);

        private void OnPastEventRemoved(GameEventBase ev) => Remove(ev.Id, _pastEventStrategies);
        private void OnCycledEventRemoved(GameEventBase ev) => Remove(ev.Id, _cycledEventStrategies);

        private void HandlePast(GameEventBase ev)
        {
            if (_pastEventStrategies.ContainsKey(ev.Id)) return;

            var strategy = _factory.GetPendingRewardGrantStrategy(ev);
            if (strategy != null)
            {
                _pastEventStrategies.Add(ev.Id, strategy);
                strategy.Complete += OnPastComplete;
                strategy.Execute();
            }
        }

        private void HandleCycled(GameEventBase ev)
        {
            if (_cycledEventStrategies.ContainsKey(ev.Id)) return;

            var strategy = _factory.GetPendingRewardGrantStrategy(ev);
            if (strategy != null)
            {
                _cycledEventStrategies.Add(ev.Id, strategy);
                strategy.Complete += OnCycledComplete;
                strategy.Execute();
            }
        }

        private void OnPastComplete(int id)
        {
            if (_pastEventStrategies.TryGetValue(id, out var strategy))
            {
                strategy.Complete -= OnPastComplete;
                strategy.Cleanup();
                _pastEventStrategies.Remove(id);
                _container.PastEventsWithPendingRewards.Remove(id);
                SaveGameRequested?.Invoke(true);
            }
        }

        private void OnCycledComplete(int id)
        {
            if (_cycledEventStrategies.TryGetValue(id, out var strategy))
            {
                strategy.Complete -= OnCycledComplete;
                strategy.Cleanup();
                _cycledEventStrategies.Remove(id);
                _container.CycledEventsWithPendingRewards.Remove(id);
                SaveGameRequested?.Invoke(true);
            }
        }

        private void UnsubscribeAndCleanup(Dictionary<int, IGameEventUnclaimedRewardGrantStrategy> strategies)
        {
            foreach (var strategy in strategies.Values)
            {
                strategy.Complete -= OnPastComplete;
                strategy.Complete -= OnCycledComplete;
                strategy.Cleanup();
            }

            strategies.Clear();
        }

        private void Remove(int id, Dictionary<int, IGameEventUnclaimedRewardGrantStrategy> strategies)
        {
            if (strategies.TryGetValue(id, out var strategy))
            {
                strategy.Complete -= OnPastComplete;
                strategy.Complete -= OnCycledComplete;
                strategy.Cleanup();
                strategies.Remove(id);
            }
        }
    }
}