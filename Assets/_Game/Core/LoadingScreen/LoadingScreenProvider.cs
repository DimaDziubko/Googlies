using System;
using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Loading;
using _Game.Utils;
using Cysharp.Threading.Tasks;

namespace _Game.Core.LoadingScreen
{
    public class LoadingScreenProvider : LocalAssetLoader, ILoadingScreenProvider
    {
        private readonly IMyLogger _logger;
        public event Action LoadingCompleted;

        public LoadingScreenProvider(IMyLogger logger)
        {
            _logger = logger;
        }
        
        public async UniTask LoadAndDestroy(ILoadingOperation loadingOperation, LoadingScreenType type)
        {
            var operations = new Queue<ILoadingOperation>();
            operations.Enqueue(loadingOperation);
            await LoadAndDestroy(operations, type);
        }

        public async UniTask LoadAndDestroy(Queue<ILoadingOperation> loadingOperations, LoadingScreenType type)
        {
            LoadingScreen loadingScreen = await Load<LoadingScreen>(AssetsConstants.LOADING_SCREEN, Constants.Scenes.UI);
            _logger.Log($"LOADING CANVAS HERE __");
            loadingScreen.Construct(_logger);
            await loadingScreen.Load(loadingOperations, type);
            LoadingCompleted?.Invoke();
            Unload();
        }
    }
}