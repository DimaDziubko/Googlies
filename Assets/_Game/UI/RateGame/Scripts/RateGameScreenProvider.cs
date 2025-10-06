using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.RateGame.Scripts
{
    public class RateGameScreenProvider : LocalAssetLoader, IRateGameProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IMyLogger _logger;

        public RateGameScreenProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IMyLogger logger)
        {
            _logger = logger;

            _cameraService = cameraService;
            _audioService = audioService;
        }


        public async UniTask<Disposable<RateGameScreen>> Load()
        {
            var popup = await LoadDisposable<RateGameScreen>(AssetsConstants.RATE_GAME_WINDOW, Constants.Scenes.UI);

            popup.Value.Construct(
                _cameraService,
                _audioService,
                _logger
                );
            return popup;
        }
    }

    public interface IRateGameProvider
    {
        UniTask<Disposable<RateGameScreen>> Load();
    }
}