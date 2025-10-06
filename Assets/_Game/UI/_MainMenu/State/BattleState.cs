using _Game.Core._Logger;
using _Game.UI._MainMenu.Scripts;
using _Game.UI._StartBattleScreen.Scripts;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._MainMenu.State
{
    public class BattleState : ILocalState
    {
        private readonly MainMenu _mainMenu;
        private readonly IStartBattleScreenProvider _provider;
        private readonly IMyLogger _logger;
        
        private Disposable<StartBattleScreen> _startBattleScreen;

        public BattleState(
            MainMenu mainMenu,
            IStartBattleScreenProvider provider,
            IMyLogger logger)
        {
            _provider = provider;
            _logger = logger;
            _mainMenu = mainMenu;
        }
        
        public async UniTask InitializeAsync() => _startBattleScreen = await _provider.Load();

        public void SetActive(bool isActive)
        {
            if (_startBattleScreen != null) 
                _startBattleScreen.Value.SetActive(isActive);
        }
        
        public void Enter()
        {
            _mainMenu.SetButtonHighlighted(MenuButtonType.Battle, true);
            
            if (_startBattleScreen != null)
            {
                _startBattleScreen.Value.Show();
            }
        }

        public void Exit()
        {
            if (_startBattleScreen?.Value != null)
            {
                _startBattleScreen.Value.Hide();
            }
            else
            {
                _logger.Log("Exit called but _startBattleScreen is null", DebugStatus.Warning);
            }
            
            _mainMenu.SetButtonHighlighted(MenuButtonType.Battle, false);
        }

        public void Cleanup()
        {
            _provider.Dispose();
            _mainMenu.SetButtonHighlighted(MenuButtonType.Battle, false);
        }
    }
}