using _Game.Core._SceneLoader;
using _Game.Core.Loading;
using _Game.Core.LoadingScreen;
using _Game.UI._MainMenu.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.GameState
{
    public class MenuState : IPayloadedState<SimpleLoadingData>
    {
        private readonly IMainMenuProvider _mainMenuProvider;
        private readonly ILoadingScreenProvider _loadingProvider;
        private readonly SceneLoader _sceneLoader;

        public MenuState(
            IMainMenuProvider mainMenuProvider,
            ILoadingScreenProvider loadingProvider,
            SceneLoader sceneLoader)
        {
            _mainMenuProvider = mainMenuProvider;
            _loadingProvider = loadingProvider;
            _sceneLoader = sceneLoader;
        }
        
        public void Enter(SimpleLoadingData data)
        {
            data.Operations.Enqueue(new MainMenuLoadingOperation(_mainMenuProvider, _sceneLoader));
            _loadingProvider.LoadAndDestroy(data.Operations, data.Type).Forget();
        }
        
        public void Exit()
        {
 
        }
    }
}
