using _Game.Core._Logger;
using _Game.UI._CardsGeneral.Scripts;
using _Game.UI._MainMenu.Scripts;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityUtils;

namespace _Game.UI._MainMenu.State
{
    public class GeneralCardsState : ILocalState
    {
        private readonly MainMenu _mainMenu;
        private readonly IGeneralCardsScreenProvider _provider;
        private readonly IMyLogger _logger;

        private readonly MenuButton _button;

        private Disposable<GeneralCardsScreen> _generalCardsScreen;
        
        public GeneralCardsState(
            MainMenu mainMenu,
            IGeneralCardsScreenProvider provider,
            IMyLogger logger)
        {
            _mainMenu = mainMenu;
            _provider = provider;
            _logger = logger;
        }

        public async UniTask InitializeAsync() => _generalCardsScreen = await _provider.Load();
        public void SetActive(bool isActive)
        {
            if (_generalCardsScreen != null) 
                _generalCardsScreen.Value.SetActive(isActive);
        }

        public void Enter()
        {
            _mainMenu.SetButtonHighlighted(MenuButtonType.Cards, true);

            if (_generalCardsScreen != null)
            {
                _generalCardsScreen.Value.Show();
                _mainMenu.CancelCardsStep();
                _mainMenu.CancelSkillsStep();
            }
        }

        public void Exit()
        {
            if (_generalCardsScreen?.Value.OrNull() != null)
            {
                _generalCardsScreen.Value.Hide();
            }
            else
            {
                _logger.Log("Exit called but _generalCardsScreen is null", DebugStatus.Warning);
            }
            
            _mainMenu.SetButtonHighlighted(MenuButtonType.Cards, false);

            _mainMenu.ShowCardsStep();
            _mainMenu.ShowSkillsStep();
        }

        public void Cleanup()
        {
            _provider.Dispose();
            _mainMenu.SetButtonHighlighted(MenuButtonType.Cards, false);
        }
    }
}