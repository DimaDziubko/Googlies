using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Services._Balancy;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay.Common;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using Balancy;
using Balancy.Data.SmartObjects;
using Balancy.Models.SmartObjects;
using Balancy.SmartObjects;
using UnityEngine;

namespace _Game.LiveopsCore
{
    public interface IGameEventScheduler
    {
        public event Action Initialized;
        public bool IsInitialized { get; }
    }

    public class GameEventScheduler :
        IGameEventScheduler,
        IDisposable,
        ISmartObjectsEvents,
        IGameScreenListener<IStartBattleScreen>
    {
        public event Action Initialized;

        private readonly IBalancySDKService _balancy;
        private readonly IUserContainer _userContainer;
        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;

        private readonly GameEventContainer _container;
        private readonly GameEventFactorySelector _selector;
        private readonly ICoroutineRunner _runner;

        private Coroutine _checkCoroutine;

        private bool _postInitDone = false;
        private bool _balancyReady = false;
        private bool _initCalled = false;
        private bool _balancyProfileReady = false;

        public bool IsInitialized { get; private set; }
        
        private IEventsSavegameReadonly Events => _userContainer.State.EventsSavegame;

        public GameEventScheduler(
            IBalancySDKService balancy,
            IUserContainer userContainer,
            GameEventContainer container,
            GameEventFactorySelector selector,
            ICoroutineRunner coroutine,
            IGameInitializer gameInitializer,
            IMyLogger logger
        )
        {
            _balancy = balancy;
            _userContainer = userContainer;
            _container = container;
            _selector = selector;
            _runner = coroutine;
            _gameInitializer = gameInitializer;
            _logger = logger;
            
            _gameInitializer.OnPostInitialization += OnPostInit;
            _balancy.Initialized += OnBalancyInit;
            _balancy.ProfileLoaded += OnBalancyProfileLoaded;
        }

        private void OnPostInit()
        {
            _postInitDone = true;
            TryInit();
        }

        private void OnBalancyInit()
        {
            _balancyReady = true;
            TryInit();
        }

        private void OnBalancyProfileLoaded()
        {
            _balancyProfileReady = true;
            TryInit();
        }

        private void TryInit()
        {
            if (_initCalled) return;
            if (!_postInitDone || !_balancyReady || !_balancyProfileReady) return;

            _initCalled = true;
            Init();
        }

        private void Init()
        {
            if (IsInitialized) return;

            _container.ActiveEvents.EventRemoved += OnEventRemoved;

            var remoteActiveEvents = _balancy.GetActiveEvents();

            if (remoteActiveEvents != null && remoteActiveEvents.Length > 0)
                SynchronizeActiveEvents(remoteActiveEvents, _userContainer.State.EventsSavegame.ActiveEvents);
            else
                RestoreLocalEvents(_userContainer.State.EventsSavegame.ActiveEvents);

            _userContainer.State.EventsSavegame.CleanupPastEvents();
            CheckPastEventsForPendingRewards(_userContainer.State.EventsSavegame.PastEvents);

            ExternalEvents.RegisterSmartObjectsListener(this);

            _logger.Log("GameEventScheduler Initialized", DebugStatus.Info);

            IsInitialized = true;
            Initialized?.Invoke();
        }

        void IDisposable.Dispose()
        {
            _gameInitializer.OnPostInitialization -= OnPostInit;
            _balancy.Initialized -= OnBalancyInit;
            _container.ActiveEvents.EventRemoved -= OnEventRemoved;
            _balancy.ProfileLoaded -= OnBalancyProfileLoaded;
        }

        private void CheckPastEventsForPendingRewards(IReadOnlyList<GameEventSavegame> saves)
        {
            List<GameEventSavegame> savesCopy = saves.ToList();

            foreach (var save in savesCopy)
            {
                CheckPastEventsForPendingReward(save);
            }
        }

        private void CheckPastEventsForPendingReward(GameEventSavegame save)
        {
            GameEventBase gameEvent = _selector.TryCreateWithPendingRewards(save);
            if (gameEvent != null)
                _container.PastEventsWithPendingRewards.Add(gameEvent);
        }

        private void RestoreLocalEvents(IReadOnlyList<GameEventSavegame> saves)
        {
            List<GameEventSavegame> savesCopy = saves.ToList();

            foreach (GameEventSavegame save in savesCopy)
            {
                RestoreLocalEvent(save);
            }
        }

        private void OnEventRemoved(GameEventBase eventBase)
        {
            if (_checkCoroutine != null)
            {
                _runner.StopCoroutine(_checkCoroutine);
                _checkCoroutine = null;
            }

            _checkCoroutine = _runner?.StartCoroutine(CheckForPendingEventsDelayed());
        }

        private void RestoreLocalEvent(GameEventSavegame save)
        {
            GameEventBase gameEvent = _selector.Create(save);
            if (gameEvent != null)
                _container.ActiveEvents.Add(gameEvent);
        }

        private IEnumerator CheckForPendingEventsDelayed()
        {
            yield return new WaitForSecondsRealtime(2f);

            var remoteActiveEvents = _balancy.GetActiveEvents();

            foreach (var remote in remoteActiveEvents)
            {
                int id = remote.GameEvent.IntUnnyId;

                if (!_container.ActiveEvents.Contains(id)
                    && Events.ActiveEvents.All(x => x.Id != id))
                {
                    _logger.Log($"[Scheduler] Adding missing active event (ID: {id}) after delay.", DebugStatus.Info);
                    ScheduleNewEvent(remote);
                }
            }
        }

        private void SynchronizeActiveEvents(
            EventInfo[] remoteActiveEvents,
            IReadOnlyList<GameEventSavegame> localActiveEvents)
        {
            _logger.Log("Synchronizing events...", DebugStatus.Warning);

            Dictionary<int, GameEventSavegame> localActiveEventsDict = new Dictionary<int, GameEventSavegame>();

            List<GameEventSavegame> markedMoveToRemove = new List<GameEventSavegame>();

            for (int i = localActiveEvents.Count - 1; i >= 0; i--)
            {
                var local = localActiveEvents[i];
                if (!localActiveEventsDict.TryAdd(local.Id, local))
                {
                    markedMoveToRemove.Add(local);
                }
            }

            foreach (var marked in markedMoveToRemove)
            {
                _userContainer.EventsStateHandler.RemoveActiveEvent(marked);
            }

            Dictionary<int, EventInfo> remoteActiveEventsDict =
                remoteActiveEvents.ToDictionary(x => x.GameEvent.IntUnnyId, x => x);


            foreach (var remoteEventId in remoteActiveEventsDict.Keys)
            {
                if (!localActiveEventsDict.TryGetValue(remoteEventId, out var value))
                {
                    ScheduleNewEvent(remoteActiveEventsDict[remoteEventId]);
                }
                else
                {
                    UpdateExistingEvent(remoteActiveEventsDict[remoteEventId], value);
                }
            }

            foreach (var localEventId in localActiveEventsDict.Keys)
            {
                if (!remoteActiveEventsDict.ContainsKey(localEventId))
                {
                    RemoveActiveEvent(localActiveEventsDict[localEventId]);
                }
            }
        }

        private void ScheduleNewEvent(EventInfo remote)
        {
            GameEventBase gameEvent = _selector.Create(remote);
            if (gameEvent != null && !_container.ActiveEvents.Contains(gameEvent.Id))
            {
                _container.ActiveEvents.Add(gameEvent);
                _userContainer.EventsStateHandler.AddActiveEvent(gameEvent.Save);
            }
        }

        private void UpdateExistingEvent(EventInfo remote, GameEventSavegame save)
        {
            GameEventBase gameEvent = _selector.Create(remote, save);
            if (gameEvent != null && !_container.ActiveEvents.Contains(gameEvent.Id))
                _container.ActiveEvents.Add(gameEvent);
        }

        private void RemoveActiveEvent(GameEventSavegame save)
        {
            _container.ActiveEvents.Remove(save.Id);
            _userContainer.EventsStateHandler.MoveToPast(save);
        }

        void ISmartObjectsEvents.OnSystemProfileConflictAppeared()
        {
        }

        void ISmartObjectsEvents.OnNewOfferActivated(OfferInfo offerInfo)
        {
        }

        void ISmartObjectsEvents.OnNewOfferGroupActivated(OfferGroupInfo offerInfo)
        {
        }

        void ISmartObjectsEvents.OnOfferDeactivated(OfferInfo offerInfo, bool wasPurchased)
        {
        }

        void ISmartObjectsEvents.OnOfferGroupDeactivated(OfferGroupInfo offerInfo, bool wasPurchased)
        {
        }

        void ISmartObjectsEvents.OnNewEventActivated(EventInfo eventInfo)
        {
            _logger.Log($"EventActivated Id: {eventInfo.GameEvent.IntUnnyId} Name: {eventInfo.GameEvent.Name}",
                DebugStatus.Info);

            if (!_container.ActiveEvents.Contains(eventInfo.GameEvent.IntUnnyId)
                && Events.ActiveEvents.All(x => x.Id != eventInfo.GameEvent.IntUnnyId))
            {
                ScheduleNewEvent(eventInfo);
            }
        }

        void ISmartObjectsEvents.OnEventDeactivated(EventInfo eventInfo)
        {
            if (_container == null || _container.ActiveEvents == null)
            {
                _logger.Log("[Scheduler] OnEventDeactivated called too early. Container not ready.",
                    DebugStatus.Warning);
                return;
            }

            if (eventInfo == null)
            {
                _logger.Log("[Scheduler] OnEventDeactivated received null eventInfo.", DebugStatus.Warning);
                return;
            }

            if (eventInfo.GameEvent == null)
            {
                _logger.Log("[Scheduler] OnEventDeactivated: eventInfo.GameEvent is null.", DebugStatus.Warning);
                return;
            }

            var id = eventInfo.GameEvent.IntUnnyId;
            var eventToRemove = _container.ActiveEvents.GetById(id);

            if (eventToRemove != null)
            {
                _runner.StartCoroutine(DelayedRemoveActiveEvent(eventToRemove.Save, 2f));
            }
        }

        private IEnumerator DelayedRemoveActiveEvent(GameEventSavegame save, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);

            if (_container == null || _container.ActiveEvents == null)
            {
                _logger.Log($"[Scheduler] DelayedRemove failed — container is null.", DebugStatus.Warning);
                yield break;
            }

            if (_container.ActiveEvents.Contains(save.Id))
            {
                _logger.Log($"[Scheduler] Delayed remove of event ID: {save.Id}", DebugStatus.Info);
                RemoveActiveEvent(save);
            }
            else
            {
                _logger.Log($"[Scheduler] Skip remove of event ID: {save.Id} — already removed.", DebugStatus.Warning);
            }
        }


        void ISmartObjectsEvents.OnOfferPurchased(OfferInfo offerInfo)
        {
        }

        void ISmartObjectsEvents.OnOfferGroupPurchased(OfferGroupInfo offerInfo, StoreItem storeItem)
        {
        }

        void ISmartObjectsEvents.OnOfferFailedToPurchase(OfferInfo offerInfo, string error)
        {
        }

        void ISmartObjectsEvents.OnStoreItemFailedToPurchase(StoreItem storeItem, string error)
        {
        }

        void ISmartObjectsEvents.OnSegmentUpdated(SegmentInfo segmentInfo)
        {
        }

        void ISmartObjectsEvents.OnUserProfilesLoaded()
        {
        }

        void ISmartObjectsEvents.OnSmartObjectsInitialized()
        {
        }

        void ISmartObjectsEvents.OnAbTestStarted(LiveOps.ABTests.TestData abTestInfo)
        {
        }

        void ISmartObjectsEvents.OnAbTestEnded(LiveOps.ABTests.TestData abTestInfo)
        {
        }

        public void OnScreenOpened(IStartBattleScreen screen)
        {
            if (_checkCoroutine != null)
            {
                _runner.StopCoroutine(_checkCoroutine);
                _checkCoroutine = null;
            }

            _checkCoroutine = _runner.StartCoroutine(CheckForPendingEventsDelayed());
        }

        void IGameScreenListener<IStartBattleScreen>.OnInfoChanged(IStartBattleScreen screen)
        {
        }

        void IGameScreenListener<IStartBattleScreen>.OnRequiresAttention(IStartBattleScreen screen)
        {
        }

        void IGameScreenListener<IStartBattleScreen>.OnScreenClosed(IStartBattleScreen screen)
        {
        }

        void IGameScreenListener<IStartBattleScreen>.OnScreenActiveChanged(IStartBattleScreen screen, bool isActive)
        {
        }

        void IGameScreenListener<IStartBattleScreen>.OnScreenDisposed(IStartBattleScreen screen)
        {
        }
    }
}