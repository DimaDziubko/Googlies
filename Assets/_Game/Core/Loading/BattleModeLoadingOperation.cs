using System;
using _Game._BattleModes.Scripts;
using _Game.Core._Logger;
using _Game.Core._SceneLoader;
using _Game.Core.Services._Camera;
using _Game.Utils;
using _Game.Utils.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;

namespace _Game.Core.Loading
{
    public sealed class BattleModeLoadingOperation : ILoadingOperation
    {
        private readonly SceneLoader _sceneLoader;
        private readonly IWorldCameraService _cameraService;
        private readonly IMyLogger _logger;

        public string Description => "Battle loading...";

        public BattleModeLoadingOperation(
            SceneLoader sceneLoader,
            IWorldCameraService cameraService,
            IMyLogger logger)
        {
            _logger = logger;
            _sceneLoader = sceneLoader;
            _cameraService = cameraService;
        }
        public async UniTask Load(Action<float> onProgress)
        {
#if UNITY_EDITOR
            var perfTimer = new PerfTimer(_logger, Description);
            perfTimer.Start();
#endif
            onProgress?.Invoke(0.5f);
            var loadOp = _sceneLoader.LoadSceneAsync(Constants.Scenes.BATTLE_MODE,
                LoadSceneMode.Additive);
            while (loadOp.isDone == false)
            {
                await UniTask.Yield();
            }
            onProgress?.Invoke(0.7f);

            var scene = SceneManager.GetSceneByName(Constants.Scenes.BATTLE_MODE);
            var sceneContext = scene.GetRoot<SceneContext>();
            var battleMode = sceneContext.Container.Resolve<BattleMode>();

            await battleMode.Initialize();

            onProgress?.Invoke(0.9f);

            _cameraService.EnableMainCamera();
            onProgress?.Invoke(1.0f);

#if UNITY_EDITOR
            perfTimer.Stop();
#endif
        }
    }
}