using _Game.Core._GameListenerComposite;
using _Game.Core.Services.UserContainer;
using _Game.UI.BattleResultPopup.Scripts;

namespace _Game.Gameplay._Battle.Scripts
{
    public class BattleStatisticsController : IEndGameListener
    {
        private readonly IUserContainer _userContainer;

        public BattleStatisticsController(IUserContainer userContainer) => 
            _userContainer = userContainer;

        public void OnEndBattle(GameResultType result, bool wasExit) => 
            _userContainer.State.BattleStatistics.AddCompletedBattle();
    }
}