using System;
using Sirenix.OdinInspector;

namespace _Game.LiveopsCore
{
    public class GameEventContainer : IDisposable
    {
        [ShowInInspector]
        public GameEventSubcontainer ActiveEvents { get; } = new();
        [ShowInInspector]
        public GameEventSubcontainer PastEventsWithPendingRewards { get; } = new();
        [ShowInInspector]
        public GameEventSubcontainer CycledEventsWithPendingRewards { get; } = new();

        void IDisposable.Dispose()
        {
            Clear();
        }

        private void Clear()
        {
            ActiveEvents.Clear();
            PastEventsWithPendingRewards.Clear();
            CycledEventsWithPendingRewards.Clear();
        }
    }
}