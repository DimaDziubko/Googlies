namespace _Game.Core._GameListenerComposite
{
    public interface IStopGameListener : IGameListener
    {
        void OnStopBattle();
    }
}