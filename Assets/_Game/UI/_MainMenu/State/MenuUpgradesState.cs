using _Game.Core._Logger;
using _Game.UI._MainMenu.Scripts;
using _Game.UI._UpgradesScreen.Scripts;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityUtils;

namespace _Game.UI._MainMenu.State
{
    public class MenuUpgradesState : ILocalState
    {
        private readonly MainMenu _mainMenu;
        private readonly IUpgradesScreenProvider _provider;
        private readonly IMyLogger _logger;
        private Disposable<UpgradesScreen> _upgradesScreen;

        public MenuUpgradesState(
            MainMenu mainMenu,
            IUpgradesScreenProvider provider,
            IMyLogger logger
            )
        {
            _mainMenu = mainMenu;
            _provider = provider;
            _logger = logger;
        }

        public async UniTask InitializeAsync() => _upgradesScreen = await _provider.Load();

        public void SetActive(bool isActive)
        {
            if (_upgradesScreen != null)
                _upgradesScreen.Value.SetActive(isActive);
        }

        public void Enter()
        {
            _mainMenu.SetButtonHighlighted(MenuButtonType.Upgrades, true);

            if (_upgradesScreen != null)
            {
                _upgradesScreen.Value.Show();

                //_mainMenu.CancelEvolveStep();
                _mainMenu.CancelUpgradesStep();
            }
        }

        public void Exit()
        {
            if (_upgradesScreen?.Value.OrNull() != null)
            {
                _upgradesScreen.Value.Hide();
            }
            else
            {
                _logger.Log("Exit called but _upgradesScreen is null", DebugStatus.Warning);
            }

            _mainMenu.SetButtonHighlighted(MenuButtonType.Upgrades, false);

            //_mainMenu.ShowEvolveStep();
            _mainMenu.ShowUpgradesStep();
        }

        public void Cleanup()
        {
            _provider.Dispose();
            _mainMenu.SetButtonHighlighted(MenuButtonType.Upgrades, false);
        }
    }
}