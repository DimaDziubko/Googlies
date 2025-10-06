namespace _Game.Core._GameListenerComposite
{
    public interface IGameSpeedListener : IGameListener
    {
        void OnBattleSpeedFactorChanged(float speedFactor);
    }
}