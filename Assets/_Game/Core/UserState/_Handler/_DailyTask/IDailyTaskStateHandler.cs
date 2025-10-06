using System;

namespace _Game.Core.UserState._Handler._DailyTask
{
    public interface IDailyTaskStateHandler
    {
        void AddCompleteDailyTask();
        void ChangeTaskIdx(int configId);
        void AddProgress(float delta);
        void ClearCompleted();
        void ChangeLastTimeGenerated(DateTime utcNow);
        void ChangeScheduledPushTime(DateTime fireTime);
    }
}