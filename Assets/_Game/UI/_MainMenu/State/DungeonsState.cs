using _Game.Core._Logger;
using _Game.UI._Dungeons.Scripts;
using _Game.UI._MainMenu.Scripts;
using _Game.UI.Global;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityUtils;

namespace _Game.UI._MainMenu.State
{
    public class DungeonsState : ILocalState
    {
        private readonly MainMenu _mainMenu;
        private readonly IDungeonsScreenProvider _provider;
        private readonly IUINotifier _uiNotifier;

        private Disposable<DungeonsScreen> _dungeonScreen;
        private readonly IMyLogger _logger;

        public DungeonsState(
            MainMenu mainMenu,
            IDungeonsScreenProvider provider,
            IMyLogger logger)
        {
            _mainMenu = mainMenu;
            _provider = provider;
            _logger = logger;
        }
        
        public async UniTask InitializeAsync() => _dungeonScreen = await _provider.Load();
        public void SetActive(bool isActive)
        {
            if (_dungeonScreen != null) 
                _dungeonScreen.Value.SetActive(isActive);
        }
        public void Enter()
        {
            _mainMenu.SetButtonHighlighted(MenuButtonType.Dungeons, true);
            
            if (_dungeonScreen != null)
            {
                _dungeonScreen.Value.Show();
            }
        }

        public void Exit()
        {
            if (_dungeonScreen?.Value.OrNull() != null)
            {
                _dungeonScreen.Value.Hide();
            }
            else
            {
                _logger.Log("Exit called but _dungeonScreen is null", DebugStatus.Warning);
            }

            _mainMenu.SetButtonHighlighted(MenuButtonType.Dungeons, false);
        }

        public void Cleanup()
        {
            _provider.Dispose();
            _mainMenu.SetButtonHighlighted(MenuButtonType.Dungeons, false);
        }
    }
}