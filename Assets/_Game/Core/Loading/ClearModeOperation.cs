using System;
using _Game._BattleModes.Scripts;
using _Game.Core.Services.Audio;
using Cysharp.Threading.Tasks;

namespace _Game.Core.Loading
{
    public class ClearModeOperation : ILoadingOperation
    {
        public string Description => "Clearing...";
        
        private readonly IGameModeCleaner _gameCleanUp;
        private readonly ISoundService _soundService;

        public ClearModeOperation(
            IGameModeCleaner gameCleanUp,
            ISoundService soundService)
        {
            _gameCleanUp = gameCleanUp;
            _soundService = soundService;
        }
        
        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0.1f);
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            onProgress?.Invoke(0.2f);
            _gameCleanUp.Cleanup();
            _soundService.Cleanup();
            
            foreach (var factory in _gameCleanUp.Factories)
            {
                factory.Cleanup();
                await factory.Unload();
            }
            
            onProgress?.Invoke(1f);
        }
    }
}