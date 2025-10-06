namespace _Game.LiveopsCore._UnclaimedRewardShowStrategies
{
    public interface IGameEventUnclaimedRewardShowStrategy
    {
        void Execute();
        void UnExecute();
        void Cleanup();
    }
}