using _Game.Core.Services.UserContainer;

namespace _Game.Core.UserState._Handler._Timeline
{
    public class TimelineStateHandler : ITimelineStateHandler
    {
        private readonly IUserContainer _userContainer;
        public TimelineStateHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }
        
        public void OpenNewAge(bool isNext = true)
        {
            _userContainer.State.TimelineState.OpenNewAge(isNext);
            _userContainer.RequestSaveGame();
        }

        public void OpenNewTimeline(bool isNext = true)
        {
            _userContainer.State.TimelineState.OpenNewTimeline(isNext);
            _userContainer.RequestSaveGame();
        }
        
        public void OpenNewBattle(int nextBattle)
        {
            _userContainer.State.TimelineState.OpenNextBattle(nextBattle);
            _userContainer.RequestSaveGame();
        }
        
        public void SetAllBattlesWon(bool allBattlesWon)
        {
            _userContainer.State.TimelineState.SetAllBattlesWon(true);
            _userContainer.RequestSaveGame();
        }
        
        public void InitLevel()
        {
            _userContainer.State.TimelineState.InitLevel();
            _userContainer.RequestSaveGame();
        }
    }
}