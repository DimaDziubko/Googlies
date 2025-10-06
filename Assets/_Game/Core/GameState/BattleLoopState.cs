using _Game.UI._MainMenu.Scripts;

namespace _Game.Core.GameState
{
    public class BattleLoopState : IState
    {
        private readonly IMainMenuProvider _mainMenuProvider;

        public BattleLoopState(IMainMenuProvider mainMenuProvider)
        {
            _mainMenuProvider = mainMenuProvider;
        }
        
        public void Enter()
        {
            _mainMenuProvider.HideMainMenu();
        }

        public void Exit()
        {
 
        }
    }
}