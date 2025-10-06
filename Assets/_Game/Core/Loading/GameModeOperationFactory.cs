using _Game._BattleModes.Scripts;
using _Game.Core._Logger;
using _Game.Core._SceneLoader;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;

namespace _Game.Core.Loading
{
    public class GameModeOperationFactory
    {
        private readonly SceneLoader _sceneLoader;
        private readonly IWorldCameraService _cameraService;
        private readonly ISoundService _soundService;
        private readonly IMyLogger _logger;

        public GameModeOperationFactory(
            SceneLoader sceneLoader,
            IWorldCameraService cameraService,
            ISoundService soundService,
            IMyLogger logger)
        {
            _sceneLoader = sceneLoader;
            _cameraService = cameraService;
            _soundService = soundService;
            _logger = logger;
        }

        public ILoadingOperation CreateBattleModeLoadingOperation() => 
            new BattleModeLoadingOperation(_sceneLoader, _cameraService, _logger);

        public ILoadingOperation CreateClearModeOperation(IGameModeCleaner cleaner) => 
            new ClearModeOperation(cleaner, _soundService);

        public ILoadingOperation CreateUnloadGameModeOperation(string battleMode) => 
            new UnloadGameModeOperation(battleMode, _cameraService);

        public ILoadingOperation CreateZombieRushModeLoadingOperation(IDungeonModel model) => 
            new ZombieRushModeLoadingOperation(_sceneLoader, _cameraService, model, _logger);
    }
}