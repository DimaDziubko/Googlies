using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._Logger;
using _Game.Core.Services.UserContainer;
using _Game.LiveopsCore._Enums;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;

namespace _Game.LiveopsCore._ShowcaseSystem
{
    public class GameEventShowcaseSystem : IGameScreenListener<IStartBattleScreen>, ISaveGameTrigger
    {
        public event Action<bool> SaveGameRequested;
        
        private readonly GameEventContainer _container;
        private readonly IMyLogger _logger;

        private bool _isBattleScreenOpened;
        private bool _isShowcaseStarted;

        private GameEventBase _currentEvent;

        private Queue<GameEventBase> _orderedQueue;


        public GameEventShowcaseSystem(GameEventContainer container,
            IMyLogger logger)
        {
            _container = container;
            _logger = logger;
        }

        private void OnShown()
        {
            _isShowcaseStarted = true;
        }

        private void OnClosed()
        {
            _isShowcaseStarted = false;

            _currentEvent.ShowcaseToken.Shown -= OnShown;
            _currentEvent.ShowcaseToken.Closed -= OnClosed;
            _currentEvent = null;

            TryToShowNext();
        }

        private void TryToStartShowcase()
        {
            if (!_isBattleScreenOpened || _isShowcaseStarted)
                return;

            IReadOnlyCollection<GameEventBase> allEvents = _container.ActiveEvents.GetAll();

            List<GameEventBase> eventsToShow = allEvents
                .Where(e => e.ShowcaseToken.ShowcaseCondition != ShowcaseCondition.Never && !e.ShowcaseToken.IsShown && !e.IsLocked)
                .OrderBy(e => e.ShowcaseToken.ShowOrder)
                .ToList();

            if (eventsToShow.Count == 0)
                return;

            _orderedQueue = new Queue<GameEventBase>(eventsToShow);

            TryToShowNext();
        }

        private void TryToShowNext()
        {
            if (_isShowcaseStarted || _orderedQueue == null || _orderedQueue.Count == 0)
                return;

            _currentEvent = _orderedQueue.Dequeue();

            _isShowcaseStarted = true;

            _logger.Log($"[Showcase] Request show: {_currentEvent.Name} (ID: {_currentEvent.Id})", DebugStatus.Info);

            _currentEvent.ShowcaseToken.Shown += OnShown;
            _currentEvent.ShowcaseToken.Closed += OnClosed;

            _currentEvent.ShowcaseToken.Request();
            
            SaveGameRequested?.Invoke(true);
        }

        public void OnScreenOpened(IStartBattleScreen screen)
        {
            _isBattleScreenOpened = true;
            TryToStartShowcase();
        }

        public void OnInfoChanged(IStartBattleScreen screen) { }

        public void OnRequiresAttention(IStartBattleScreen screen) { }


        public void OnScreenClosed(IStartBattleScreen screen)
        {
            _isBattleScreenOpened = false;
        }

        public void OnScreenActiveChanged(IStartBattleScreen screen, bool isActive) 
        {
            if(!isActive) _isBattleScreenOpened = false;
        }

        public void OnScreenDisposed(IStartBattleScreen screen) { }
    }
}