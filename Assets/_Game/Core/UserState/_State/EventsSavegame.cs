using System;
using System.Collections.Generic;
using System.Linq;

namespace _Game.Core.UserState._State
{
    public class EventsSavegame : IEventsSavegameReadonly
    {
        public List<GameEventSavegame> ActiveEvents;
        public List<GameEventSavegame> PastEvents;

        IReadOnlyList<GameEventSavegame> IEventsSavegameReadonly.ActiveEvents => ActiveEvents;
        IReadOnlyList<GameEventSavegame> IEventsSavegameReadonly.PastEvents => PastEvents;

        public void AddActiveEvent(GameEventSavegame gameEvent)
        {
            if (gameEvent == null) return;
            ActiveEvents.Add(gameEvent);
        }

        public bool RemoveActiveEvent(GameEventSavegame gameEvent)
        {
            if (gameEvent == null) return false;

            return ActiveEvents.Remove(gameEvent);
        }

        public void AddPastEvent(GameEventSavegame gameEvent)
        {
            if (gameEvent == null) return;

            PastEvents.Add(gameEvent);
        }

        public bool RemovePastEvent(GameEventSavegame gameEvent)
        {
            if (gameEvent == null) return false;

            return PastEvents.Remove(gameEvent);
        }

        public void MoveToPast(GameEventSavegame gameEvent)
        {
            if (gameEvent == null) return;

            if (RemoveActiveEvent(gameEvent))
            {
                AddPastEvent(gameEvent);
            }
        }

        public void CleanupPastEvents(int maxCount = 5, int maxDays = 30)
        {
            PastEvents = PastEvents
                .Where(e => (DateTime.UtcNow - e.EndDateUtc).TotalDays <= maxDays)
                .OrderByDescending(e => e.EndDateUtc)
                .Take(maxCount)
                .ToList();
        }
    }
}