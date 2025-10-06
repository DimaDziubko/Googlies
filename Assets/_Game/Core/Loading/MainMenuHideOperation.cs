using System;
using _Game.UI._MainMenu.Scripts;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public class MainMenuHideOperation : ILoadingOperation
    {
        private readonly IMainMenuProvider _mainMenuProvider;
        private readonly Type _targetState;
        public string Description => "Hiding menu...";

        public MainMenuHideOperation(IMainMenuProvider mainMenuProvider)
        {
            _mainMenuProvider = mainMenuProvider;
        }
        
        public UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0);
            _mainMenuProvider.HideMainMenu();
            //_mainMenuProvider.Dispose();
            onProgress?.Invoke(1);
            return UniTask.CompletedTask;
        }
    }
}