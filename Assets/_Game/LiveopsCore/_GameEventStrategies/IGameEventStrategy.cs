using System;

namespace _Game.LiveopsCore._GameEventStrategies
{
    public interface IGameEventStrategy
    {
        event Action<GameEventBase> Complete;
        void Execute();
        void UnExecute();
        void Cleanup();
    }
    
    public interface ICycleGameEventStrategy : IGameEventStrategy
    {
        event Action<int> CycleChanged;
    }
    
    public interface ISubCycleGameEventStrategy : ICycleGameEventStrategy
    {
        event Action<int> SubCycleChanged;
    }
}