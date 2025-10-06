using System;

namespace _Game.Core.UserState._State
{
    public interface IFreeGemsPackStateReadonly
    {
        event Action<int, int> FreeGemsPackCountChanged;
        int FreeGemPackCount { get; }
        DateTime LastFreeGemPackDay { get;}
    }
}