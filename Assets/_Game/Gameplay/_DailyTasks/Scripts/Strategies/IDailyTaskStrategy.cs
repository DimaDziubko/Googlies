using System;

namespace _Game.Gameplay._DailyTasks.Scripts.Strategies
{
    public interface IDailyTaskStrategy
    {
        event Action Completed;
        void Initialize();
        void Execute();
        void Dispose();
    }
}