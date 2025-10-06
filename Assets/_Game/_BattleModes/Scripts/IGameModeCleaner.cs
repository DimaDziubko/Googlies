using System.Collections.Generic;
using _Game.Core.Factory;

namespace _Game._BattleModes.Scripts
{
    public interface IGameModeCleaner
    {
        IEnumerable<GameObjectFactory> Factories { get; }
        string SceneName { get; }
        void Cleanup();
    }
}