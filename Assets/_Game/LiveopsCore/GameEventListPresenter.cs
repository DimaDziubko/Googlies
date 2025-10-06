using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.UI._StartBattleScreen.Scripts;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using Sirenix.OdinInspector;
using UnityUtils;
using Zenject;

namespace _Game.LiveopsCore
{
    public class GameEventListPresenter :
        IInitializable,
        IDisposable,
        IGameScreenListener<IStartBattleScreen>
    {
        private readonly GameEventContainer _container;
        private readonly IStartBattleScreenProvider _screenProvider;

        [ShowInInspector, ReadOnly]
        private readonly Dictionary<int, GameEventPresenterBase> _activeEvents = new();

        private readonly GameEventPresenterFactory _factory;
        private readonly IMyLogger _logger;

        private StartBattleScreen BattleScreen => _screenProvider.GetScreen();

        public GameEventListPresenter(
            GameEventContainer container,
            IStartBattleScreenProvider screenProvider,
            GameEventPresenterFactory factory,
            IMyLogger logger)
        {
            _container = container;
            _screenProvider = screenProvider;
            _factory = factory;
            _logger = logger;
        }

        void IInitializable.Initialize()
        {
            _container.ActiveEvents.EventAdded += OnEventAdded;
            _container.ActiveEvents.EventRemoved += OnEventRemoved;
        }

        void IDisposable.Dispose()
        {
            _container.ActiveEvents.EventAdded -= OnEventAdded;
            _container.ActiveEvents.EventRemoved -= OnEventRemoved;
        }

        private void OnEventAdded(GameEventBase gameEvent)
        {
            _logger.Log($"Game event added Id: {gameEvent.Id} Name: {gameEvent.Name}", DebugStatus.Info);
            CreateOrRefresh(gameEvent);
        }

        private void OnEventRemoved(GameEventBase gameEvent)
        {
            if (_activeEvents.TryGetValue(gameEvent.Id, out var @event))
            {
                if (BattleScreen.OrNull() != null)
                    BattleScreen.GetGameEventPanel(@event.Model.PanelType).RemoveElement(@event.Model.SlotType, @event.View);

                @event.Dispose();
                _activeEvents.Remove(gameEvent.Id);
            }
        }

        void IGameScreenListener<IStartBattleScreen>.OnScreenOpened(IStartBattleScreen screen)
        {
            foreach (var gameEvent in _container.ActiveEvents.GetAll()) 
                CreateOrRefresh(gameEvent);
            
            if (BattleScreen.OrNull() != null) 
                Sort();
            
            
        }

        private void CreateOrRefresh(GameEventBase gameEvent)
        {
            if (_activeEvents.TryGetValue(gameEvent.Id, out var @event))
            {
                RefreshEvent(gameEvent, @event);
            }
            else
            {
                CreateNewEvent(gameEvent);
            }
        }

        private void CreateNewEvent(GameEventBase gameEvent)
        {
            _logger.Log($"Create new event presenter Id: {gameEvent.Id} Name: {gameEvent.Name}", DebugStatus.Info);

            var battleScreen = BattleScreen.OrNull();
            if (battleScreen == null || gameEvent.IsOnBreak) return;

            var panel = battleScreen.GetGameEventPanel(gameEvent.PanelType);
            if (panel == null) return;

            var view = panel.SpawnElement(gameEvent.SlotType);
            if (view == null)
            {
                _logger.Log($"Failed to spawn view for event {gameEvent.Id} ({gameEvent.Name})", DebugStatus.Warning);
                return;
            }

            var presenter = _factory.CreatePresenter(gameEvent, view);
            if (presenter != null)
            {
                _activeEvents.Add(gameEvent.Id, presenter);
                presenter.Initialize();
            }
            else
            {
                _logger.Log($"Failed to create presenter for event {gameEvent.Id} ({gameEvent.Name})", DebugStatus.Warning);
                panel.RemoveElement(gameEvent.SlotType, view);
            }
        }

        private void RefreshEvent(GameEventBase gameEvent, GameEventPresenterBase activeEvent)
        {
            _logger.Log($"Refresh event presenter Id: {gameEvent.Id} Name: {gameEvent.Name}", DebugStatus.Info);

            if (BattleScreen.OrNull() != null && !gameEvent.IsOnBreak)
            {
                var view = activeEvent.View;

                if (view.OrNull() == null)
                {
                    view = BattleScreen.GetGameEventPanel(gameEvent.PanelType).SpawnElement(gameEvent.SlotType);
                }

                //view.SetupPin(gameEvent.PanelType);
                activeEvent.SetGameEvent(gameEvent);
                activeEvent.SetView(view);
                activeEvent.Initialize();
            }
        }

        private void Sort()
        {
            List<GameEventPresenterBase> sortedEvents = _activeEvents.Values
                .OrderBy(p => p.Model.SortOrder)
                .ThenBy(p => p.View.GetSiblingIndex())
                .ToList();

            for (int i = 0; i < sortedEvents.Count; i++)
            {
                sortedEvents[i].View.SetSiblingIndex(i);
            }
        }

        void IGameScreenListener<IStartBattleScreen>.OnInfoChanged(IStartBattleScreen screen) { }
        void IGameScreenListener<IStartBattleScreen>.OnRequiresAttention(IStartBattleScreen screen) { }

        void IGameScreenListener<IStartBattleScreen>.OnScreenClosed(IStartBattleScreen screen)
        {
            Unsubscribe();
        }

        void IGameScreenListener<IStartBattleScreen>.OnScreenActiveChanged(IStartBattleScreen screen, bool isActive) { }

        void IGameScreenListener<IStartBattleScreen>.OnScreenDisposed(IStartBattleScreen screen)
        {
            _logger.Log("START SCREEN DISPOSED GAME EVENT LIST PRESENTER", DebugStatus.Info);

            Dispose();
        }

        private void Unsubscribe()
        {
            foreach (GameEventPresenterBase presenter in _activeEvents.Values)
            {
                presenter.Dispose();
            }
        }

        private void Dispose()
        {
            foreach (GameEventPresenterBase presenter in _activeEvents.Values)
            {
                presenter.Dispose();

                if (BattleScreen.OrNull() != null)
                {
                    BattleScreen.GetGameEventPanel(presenter.Model.PanelType)
                        .RemoveElement(presenter.Model.SlotType, presenter.View);
                }
            }

            if (BattleScreen.OrNull() != null)
            {
                BattleScreen.ClearEventPanels();
            }
        }
    }
}