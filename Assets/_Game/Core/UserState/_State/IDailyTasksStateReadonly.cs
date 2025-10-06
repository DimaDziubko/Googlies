using System;
using System.Collections.Generic;

namespace _Game.Core.UserState._State
{
    public interface IDailyTasksStateReadonly
    {
        event Action TaskCompletedChanged;
        
        List<int> CompletedTasks { get; }
        DateTime LastTimeGenerated { get; }
        DateTime ScheduledPushTime { get; }
        int CurrentTaskIdx { get; }
        float ProgressOnTask { get; }
    }
}