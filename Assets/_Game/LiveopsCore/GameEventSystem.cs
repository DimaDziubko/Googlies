using System;
using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using _Game.LiveopsCore._GameEventStrategies;
using Zenject;

namespace _Game.LiveopsCore
{
    public class GameEventSystem : 
        IInitializable,
        IDisposable,
        ISaveGameTrigger
    {
        public event Action<bool> SaveGameRequested;
        
        private readonly GameEventContainer _container;
        private readonly GameEventStrategyFactory _factory;
        private readonly IMyLogger _logger;
        private readonly IUserContainer _userContainer;

        private readonly Dictionary<int, IGameEventStrategy> _activeEvents = new();

        public GameEventSystem(
            GameEventContainer container,
            GameEventStrategyFactory factory,
            IUserContainer userContainer,
            IMyLogger logger)
        {
            _logger = logger;
            _container = container;
            _factory = factory;
            _userContainer = userContainer;
        }

        void IInitializable.Initialize()
        {
            _container.ActiveEvents.EventAdded += OnEventAdded;
            _container.ActiveEvents.EventRemoved += OnEventRemoved;
        }

        private void OnEventAdded(GameEventBase eventBase)
        {
            var eventStrategy = _factory.GetStrategy(eventBase);
            if (eventStrategy != null)
            {
                eventStrategy.Complete += OnComplete;
                
                if (eventStrategy is ICycleGameEventStrategy cycleStrategy)
                {
                    cycleStrategy.CycleChanged += OnCycleChanged;
                    
                    if (IsCycleActiveEventWithUnclaimedRewards(eventBase.Id, out var model))
                    {
                        if(!_container.CycledEventsWithPendingRewards.Contains(model.Id))
                            _container.CycledEventsWithPendingRewards.Add(model);    
                    }
                }
                
                if (eventStrategy is ISubCycleGameEventStrategy subCycleStrategy)
                {
                    subCycleStrategy.CycleChanged += OnCycleChanged;
                    subCycleStrategy.SubCycleChanged += OnCycleChanged;
                    
                    if (IsCycleActiveEventWithUnclaimedRewards(eventBase.Id, out var model))
                    {
                        if(!_container.CycledEventsWithPendingRewards.Contains(model.Id))
                            _container.CycledEventsWithPendingRewards.Add(model);    
                    }
                }
                
                _activeEvents.TryAdd(eventBase.Id, eventStrategy);
                _logger.Log($"GAME EVENT SYSTEM EventAdded {eventBase.Id}", DebugStatus.Info);
                
                eventStrategy.Execute();
            }
            
            SaveGameRequested?.Invoke(true);
        }

        private void OnEventRemoved(GameEventBase eventBase)
        {
            if (_activeEvents.TryGetValue(eventBase.Id, out var strategy))
            {
                strategy.UnExecute();
                strategy.Cleanup();
                _activeEvents.Remove(eventBase.Id);
                
                if (eventBase.HasUnclaimedRewards()) 
                    _container.PastEventsWithPendingRewards.Add(eventBase);
                
                _userContainer.EventsStateHandler.MoveToPast(eventBase.Save);
                
                _logger.Log($"GAME EVENT SYSTEM EventRemoved {eventBase.Id}", DebugStatus.Info);
            }
        }

        void IDisposable.Dispose()
        {
            _container.ActiveEvents.EventAdded -= OnEventAdded;
            _container.ActiveEvents.EventRemoved -= OnEventRemoved;

            foreach (var activeEvent in _activeEvents.Values)
            {
                activeEvent.Complete -= OnComplete;
                
                if (activeEvent is ICycleGameEventStrategy cycleStrategy)
                {
                    cycleStrategy.CycleChanged -= OnCycleChanged;
                }

                if (activeEvent is ISubCycleGameEventStrategy subCycleStrategy)
                {
                    subCycleStrategy.CycleChanged -= OnCycleChanged;
                    subCycleStrategy.SubCycleChanged -= OnCycleChanged;
                }
                
                activeEvent.Cleanup();
            }
            
            _activeEvents.Clear();
        }

        private void OnComplete(GameEventBase eventBase)
        {
            if (_activeEvents.TryGetValue(eventBase.Id, out var strategy))
            {
                strategy.Cleanup();
                strategy.Complete -= OnComplete;

                if (strategy is ICycleGameEventStrategy cycleStrategy)
                {
                    cycleStrategy.CycleChanged -= OnCycleChanged;
                }

                if (strategy is ISubCycleGameEventStrategy subCycleStrategy)
                {
                    subCycleStrategy.CycleChanged -= OnCycleChanged;
                    subCycleStrategy.SubCycleChanged -= OnCycleChanged;
                }

                if (eventBase.HasUnclaimedRewards()) 
                    _container.PastEventsWithPendingRewards.Add(eventBase);
                
                _activeEvents.Remove(eventBase.Id);
                _container.ActiveEvents.Remove(eventBase.Id);
                _userContainer.EventsStateHandler.MoveToPast(eventBase.Save);
                _logger.Log($"GAME EVENT SYSTEM EventCompleted {eventBase.Id}", DebugStatus.Info);
            }
            
            SaveGameRequested?.Invoke(true);
        }

        private void OnCycleChanged(int id)
        {
            if (IsCycleActiveEventWithUnclaimedRewards(id, out var model))
            {
                _container.CycledEventsWithPendingRewards.Add(model);
            }
            
            SaveGameRequested?.Invoke(true);
        }
        
        private bool IsCycleActiveEventWithUnclaimedRewards(int id, out GameEventBase model)
        {
            model = _container.ActiveEvents.GetById(id);
            return model != null && model.HasUnclaimedRewards();
        }
    }
}