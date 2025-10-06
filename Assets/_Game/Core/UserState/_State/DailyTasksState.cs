using System;
using System.Collections.Generic;

namespace _Game.Core.UserState._State
{
    public interface IDailyTask
    {
        float ProgressOnTask { get;}
        public event Action ProgressChanged;
        void AddProgress(float delta);
        void ResetProgress();
    }

    public class DailyTasksState : IDailyTasksStateReadonly, IDailyTask
    {
        public float ProgressOnTask;
        public int CurrentTaskIdx;
        public List<int> CompletedTasks;
        public DateTime LastTimeGenerated;
        public DateTime ScheduledPushTime;
        
        public event Action ProgressChanged;
        public event Action TaskCompletedChanged;

        List<int> IDailyTasksStateReadonly.CompletedTasks => CompletedTasks;
        DateTime IDailyTasksStateReadonly.LastTimeGenerated => LastTimeGenerated;
        int IDailyTasksStateReadonly.CurrentTaskIdx => CurrentTaskIdx;
        float IDailyTasksStateReadonly.ProgressOnTask => ProgressOnTask;
        DateTime IDailyTasksStateReadonly.ScheduledPushTime => ScheduledPushTime;
        float IDailyTask.ProgressOnTask => ProgressOnTask;

        public void ChangeLastTimeGenerated(DateTime time)
        {
            LastTimeGenerated = time;
        }
        
        public void ChangePushTime(DateTime time)
        {
            ScheduledPushTime = time;
        }


        public void ChangeCurrentTaskIdx(int newIdx)
        {
            CurrentTaskIdx = newIdx;
        }
        
        public void CompleteTask()
        {
            CompletedTasks.Add(CurrentTaskIdx);
            AddProgress(-ProgressOnTask);
            TaskCompletedChanged?.Invoke();
        }

        public void AddProgress(float delta)
        {
            ProgressOnTask += delta;
            ProgressChanged?.Invoke();
        }

        public void ResetProgress()
        {
            ProgressOnTask = 0;
            ProgressChanged?.Invoke();
        }

        public void ClearCompleted() => 
            CompletedTasks.Clear();

        public void ClearProgress() => 
            ProgressOnTask = 0;
    }
}