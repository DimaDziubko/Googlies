namespace _Game.Core._GameListenerComposite
{
    public interface IPauseListener : IGameListener
    {
        void SetPaused(bool isPaused);
    }
}