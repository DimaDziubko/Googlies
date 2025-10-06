using System;
using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._MainMenu.Scripts
{
    public class MainMenuProvider : LocalAssetLoader, IMainMenuProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly MainMenu _mainMenu;

        private Disposable<MainMenuScreen> _mainMenuScreen;

        public MainMenuProvider(
            IWorldCameraService cameraService,
            MainMenu mainMenu)
        {
            _cameraService = cameraService;
            _mainMenu = mainMenu;
        }
        public async UniTask<Disposable<MainMenuScreen>> Load()
        {
            if (_mainMenuScreen != null) return _mainMenuScreen;
            _mainMenuScreen = await LoadDisposable<MainMenuScreen>(AssetsConstants.MAIN_MENU, Constants.Scenes.UI);
            
            _mainMenuScreen.Value.Construct(
                _cameraService,
                _mainMenu);
            
            return _mainMenuScreen;
        }
        
        public void Dispose()
        {
            if (_mainMenuScreen != null)
            {
                _mainMenuScreen.Dispose();
                _mainMenuScreen = null;
            }
        }

        public void ShowMainMenu(Type targetState)
        {
            if (_mainMenuScreen != null)
            {
                _mainMenuScreen.Value.Show();
            }
        }
        
        public void HideMainMenu()
        {
            if (_mainMenuScreen != null)
            {
                _mainMenuScreen.Value.Hide();
            }
        }
    }
}