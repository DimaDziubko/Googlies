using System.Collections.Generic;

namespace _Game.Core.UserState._State
{
    public interface IEventsSavegameReadonly
    {
        public IReadOnlyList<GameEventSavegame> ActiveEvents { get; }
        public IReadOnlyList<GameEventSavegame> PastEvents { get; }
    }
}