using System;

namespace _Game.Core.UserState._State
{
    public interface ITasksStateReadonly
    {
        int TotalCompletedTasks { get; }
        event Action Changed;
    }
}