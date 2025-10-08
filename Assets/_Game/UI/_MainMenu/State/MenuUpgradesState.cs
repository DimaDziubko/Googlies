using _Game.Core._Logger;
using _Game.UI._MainMenu.Scripts;
using _Game.UI._UpgradesAndEvolution.Scripts;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityUtils;

namespace _Game.UI._MainMenu.State
{
    public class MenuUpgradesState : ILocalState
    {
        private readonly MainMenu _mainMenu;
        private readonly IUpgradeAndEvolutionScreenProvider _provider;
        private readonly IMyLogger _logger;

        private Disposable<UpgradeAndEvolutionScreen> _upgradesAndEvolutionScreen;

        public MenuUpgradesState(
            MainMenu mainMenu,
            IUpgradeAndEvolutionScreenProvider provider,
            IMyLogger logger
            )
        {
            _mainMenu = mainMenu;
            _provider = provider;
            _logger = logger;
        }
        
        public async UniTask InitializeAsync() => _upgradesAndEvolutionScreen = await _provider.Load();

        public void SetActive(bool isActive)
        {
            if (_upgradesAndEvolutionScreen != null) 
                _upgradesAndEvolutionScreen.Value.SetActive(isActive);
        }
        
        public void Enter()
        {
            _mainMenu.SetButtonHighlighted(MenuButtonType.Upgrades, true);

            if (_upgradesAndEvolutionScreen != null)
            {
                _upgradesAndEvolutionScreen.Value.Show();
                
                _mainMenu.CancelEvolveStep();
                _mainMenu.CancelUpgradesStep();
            }
        }

        public void Exit()
        {
            if (_upgradesAndEvolutionScreen?.Value.OrNull() != null)
            {
                _upgradesAndEvolutionScreen.Value.Hide();
            }
            else
            {
                _logger.Log("Exit called but _upgradesAndEvolutionScreen is null", DebugStatus.Warning);
            }
            
            _mainMenu.SetButtonHighlighted(MenuButtonType.Upgrades, false);
            
            _mainMenu.ShowEvolveStep();
            _mainMenu.ShowUpgradesStep();
        }

        public void Cleanup()
        {
            _provider.Dispose();
            _mainMenu.SetButtonHighlighted(MenuButtonType.Upgrades, false);
        }
    }
}