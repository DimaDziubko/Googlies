using System;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Utils;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public class InitializationOperation : ILoadingOperation
    {
        public string Description => "Initialization...";

        private readonly IGameInitializer _gameInitializer;
        private readonly IMyLogger _logger;

        public InitializationOperation(
            IGameInitializer gameInitializer,
            IMyLogger logger)
        {
            _gameInitializer = gameInitializer;
            _logger = logger;

        }

        public async UniTask Load(Action<float> onProgress)
        {
#if UNITY_EDITOR
            var perfTimer = new PerfTimer(_logger, Description);
            perfTimer.Start();
#endif
            onProgress.Invoke(0.05f);
            await _gameInitializer.InitAsync();
            onProgress.Invoke(0.5f);
            _gameInitializer.Init();
            onProgress.Invoke(1.0f);

#if UNITY_EDITOR
            perfTimer.Stop();
#endif
        }
    }
}