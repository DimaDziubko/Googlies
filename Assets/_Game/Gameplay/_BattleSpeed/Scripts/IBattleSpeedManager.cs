using _Game.Core._GameListenerComposite;

namespace _Game.Gameplay._BattleSpeed.Scripts
{
    public interface IBattleSpeedManager
    {
        float CurrentSpeedFactor { get; }
        void Register(IGameSpeedListener listener);
        void Unregister(IGameSpeedListener listener);
        void SetSpeedFactor(float speedFactor);
    }
}