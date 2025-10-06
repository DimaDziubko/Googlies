using System;
using _Game.Core.Services._Camera;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace _Game.Core.Loading
{
    public sealed class UnloadGameModeOperation : ILoadingOperation
    {
        private readonly string _sceneName;
        public string Description => "Unloading...";

        private readonly IWorldCameraService _cameraService;

        public UnloadGameModeOperation(
            string sceneName,
            IWorldCameraService cameraService)
        {
            _sceneName = sceneName;
            _cameraService = cameraService;
        }

        public async UniTask Load(Action<float> onProgress)
        {
            onProgress?.Invoke(0.2f);

            var unloadOp = SceneManager.UnloadSceneAsync(_sceneName);
            
            while (unloadOp.isDone == false)
            {
                await UniTask.Yield();
            }
            
            _cameraService.DisableMainCamera();
            
            onProgress?.Invoke(1f);
        }
    }
}