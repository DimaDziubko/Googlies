using System;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.UserState._Handler._DailyTask
{
    public class DailyTaskStateHandler : IDailyTaskStateHandler
    {
        private readonly IUserContainer _userContainer;

        public DailyTaskStateHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }

        public void AddCompleteDailyTask()
        {
            _userContainer.State.DailyTasksState.CompleteTask();
            _userContainer.State.TasksState.AddCompletedTask();
            _userContainer.RequestSaveGame();

        }

        public void ChangeTaskIdx(int id)
        {
            _userContainer.State.DailyTasksState.ChangeCurrentTaskIdx(id);
            _userContainer.RequestSaveGame();
        }

        public void AddProgress(float delta)
        {
            _userContainer.State.DailyTasksState.AddProgress(delta);
            _userContainer.RequestSaveGame();
        }

        public void ClearCompleted()
        {
            _userContainer.State.DailyTasksState.ClearCompleted();
            _userContainer.RequestSaveGame();

        }

        public void ChangeLastTimeGenerated(DateTime newTime)
        {
            _userContainer.State.DailyTasksState.ChangeLastTimeGenerated(newTime);
            _userContainer.RequestSaveGame();
        }

        public void ChangeScheduledPushTime(DateTime fireTime)
        {
            _userContainer.State.DailyTasksState.ChangePushTime(fireTime);
            _userContainer.RequestSaveGame();
        }
    }
}