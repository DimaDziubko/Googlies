using _Game.UI.Common.Scripts;

namespace _Game.UI.Global
{
    public interface IGameScreenListener<TScreen> : IGameScreenListener where TScreen : IGameScreen
    {
        void OnScreenOpened(TScreen screen);
        void OnInfoChanged(TScreen screen);
        void OnRequiresAttention(TScreen screen);
        void OnScreenClosed(TScreen screen);
        void OnScreenActiveChanged(TScreen screen, bool isActive);
        void OnScreenDisposed(TScreen screen);
    }

    public interface IGameScreenListener
    {
    }
}