using System;

namespace _Game.Core.UserState._State
{
    public class TasksState : ITasksStateReadonly
    {
        public int TotalCompletedTasks;
        int ITasksStateReadonly.TotalCompletedTasks => TotalCompletedTasks;
        public event Action Changed;

        public void AddCompletedTask()
        {
            TotalCompletedTasks++;
            Changed?.Invoke();
        }
    }
}