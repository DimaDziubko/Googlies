using System;
using _Game.UI.Common.Scripts;
using _Game.UI.Pin.Scripts;

namespace _Game.UI.Global
{
    public interface IUINotifier
    {
        void Register(IGameScreenListener listener);
        void Unregister(IGameScreenListener listener);
        void RegisterScreen<TScreen>(TScreen screen, IGameScreenEvents<TScreen> screenEvents)
            where TScreen : IGameScreen;
        void UnregisterScreen<TScreen>(IGameScreenEvents<TScreen> screenEvents) where TScreen : IGameScreen;
        
        void RegisterPin(Type screenType, PinView pin);
        void UnregisterPin(Type screenType);
    }
}