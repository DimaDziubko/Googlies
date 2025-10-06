using System;
using _Game.Core._SceneLoader;
using _Game.UI._MainMenu.Scripts;
using _Game.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace _Game.Core.Loading
{
    public class MainMenuLoadingOperation : ILoadingOperation
    {
        private readonly IMainMenuProvider _provider;
        private readonly Type _targetState;
        private readonly SceneLoader _sceneLoader;
        public string Description => "Loading menu...";

        public MainMenuLoadingOperation(
            IMainMenuProvider provider,
            SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
            _provider = provider;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0);
            
            Scene startup = SceneManager.GetSceneByName(Constants.Scenes.STARTUP);
            
            if (startup.IsValid() && startup.isLoaded)
            {
                _sceneLoader.UnloadSceneAsync(Constants.Scenes.STARTUP);
            }
            
            await _provider.Load();
            _provider.ShowMainMenu(_targetState);
            
            onProgress?.Invoke(1);
        }
    }
}