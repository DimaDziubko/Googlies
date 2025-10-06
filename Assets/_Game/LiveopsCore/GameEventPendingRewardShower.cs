using System;
using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.LiveopsCore._UnclaimedRewardShowStrategies;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using Sirenix.OdinInspector;
using Zenject;

namespace _Game.LiveopsCore
{
    public class GameEventPendingRewardShower :
        IInitializable,
        IDisposable,
        IGameScreenListener<IStartBattleScreen>
    {
        private readonly GameEventContainer _container;
        private readonly GameEventStrategyFactory _factory;

        private readonly Dictionary<int, IGameEventUnclaimedRewardShowStrategy> _pastEventStrategies = new();
        private readonly Dictionary<int, IGameEventUnclaimedRewardShowStrategy> _cycledEventStrategies = new();

        [ShowInInspector, ReadOnly]
        private bool _isBattleScreenOpened;
        [ShowInInspector, ReadOnly]
        private bool _isBattleScreenActive;

        private readonly IMyLogger _logger;

        public GameEventPendingRewardShower(
            GameEventContainer container,
            GameEventStrategyFactory factory,
            IMyLogger logger)
        {
            _logger = logger;
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

            var strategy = _factory.GetPendingRewardShowStrategy(ev);
            if (strategy != null)
            {
                _pastEventStrategies.Add(ev.Id, strategy);
                
                if (_isBattleScreenOpened && _isBattleScreenActive)
                {
                    strategy.Execute();
                }
            }
        }

        private void HandleCycled(GameEventBase ev)
        {
            if (_cycledEventStrategies.ContainsKey(ev.Id)) return;

            var strategy = _factory.GetPendingRewardShowStrategy(ev);
            
            if (strategy != null)
            {
                _cycledEventStrategies.Add(ev.Id, strategy);

                if (_isBattleScreenOpened && _isBattleScreenActive)
                {
                    strategy.Execute();
                }
            }
        }
        
        private void UnsubscribeAndCleanup(Dictionary<int, IGameEventUnclaimedRewardShowStrategy> strategies)
        {
            foreach (var strategy in strategies.Values)
            {
                strategy.Cleanup();
            }

            strategies.Clear();
        }

        private void Remove(int id, Dictionary<int, IGameEventUnclaimedRewardShowStrategy> strategies)
        {
            if (strategies.TryGetValue(id, out var strategy))
            {
                strategy.Cleanup();
                strategies.Remove(id);
            }
        }

        void IGameScreenListener<IStartBattleScreen>.OnScreenOpened(IStartBattleScreen screen)
        {
            _isBattleScreenOpened = true;
            _isBattleScreenActive = true;
            
            foreach (var strategy in _cycledEventStrategies.Values) 
                strategy.Execute();
            
            foreach (var strategy in _pastEventStrategies.Values) 
                strategy.Execute();
        }

        void IGameScreenListener<IStartBattleScreen>.OnInfoChanged(IStartBattleScreen screen) { }
        void IGameScreenListener<IStartBattleScreen>.OnRequiresAttention(IStartBattleScreen screen) { }
        void IGameScreenListener<IStartBattleScreen>.OnScreenClosed(IStartBattleScreen screen)
        {
            _isBattleScreenOpened = false;
            Cleanup();
        }

        void IGameScreenListener<IStartBattleScreen>.OnScreenActiveChanged(IStartBattleScreen screen, bool isActive)
        {
            _logger.Log("GAME EVENT PENDING REWARD SHOWER ON BATTLE SCREEN ACTIVE CHANGED", DebugStatus.Info);
            
            _isBattleScreenActive = isActive;

            if (_isBattleScreenActive && _isBattleScreenOpened)
            {
                foreach (var strategy in _cycledEventStrategies.Values) 
                    strategy.Execute();
            
                foreach (var strategy in _pastEventStrategies.Values) 
                    strategy.Execute();
            }
        }

        void IGameScreenListener<IStartBattleScreen>.OnScreenDisposed(IStartBattleScreen screen)
        {
            _isBattleScreenOpened = false;
            _isBattleScreenActive = false;
            
            Cleanup();
        }

        private void Cleanup()
        {
            foreach (var strategy in _cycledEventStrategies.Values)
            {
                strategy.UnExecute();
                strategy.Cleanup();
            }

            foreach (var strategy in _pastEventStrategies.Values)
            {
                strategy.UnExecute();
                strategy.Cleanup();
            }
        }
    }
}