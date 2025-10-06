using System;

namespace _Game.UI.Common.Scripts
{
    public interface IGameScreenEvents<out TScreen> where TScreen : IGameScreen
    {
        event Action<TScreen> ScreenOpened;
        event Action<TScreen> InfoChanged;
        event Action<TScreen> RequiresAttention;
        event Action<TScreen> ScreenClosed;
        event Action<TScreen, bool> ActiveChanged;
        event Action<TScreen> ScreenDisposed;
    }
}