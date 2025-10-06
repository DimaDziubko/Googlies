using System;

namespace _Game.LiveopsCore._GrantStrategies
{
    public interface IGameEventUnclaimedRewardGrantStrategy
    {
        event Action<int> Complete;
        bool Execute();
        void UnExecute();
        void Cleanup();
    }
}