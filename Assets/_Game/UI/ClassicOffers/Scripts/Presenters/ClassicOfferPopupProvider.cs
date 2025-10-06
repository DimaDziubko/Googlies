using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.UI.ClassicOffers.DailyChallenge.Scripts;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.ClassicOffers.Scripts.Presenters
{
    public class ClassicOfferPopupProvider : LocalAssetLoader
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;

        public ClassicOfferPopupProvider(
            IWorldCameraService cameraService,
            IAudioService audioService
        )
        {
            _cameraService = cameraService;
            _audioService = audioService;
        }

        public async UniTask<Disposable<ClassicOfferPopup>> Load()
        {
            var popup = await LoadDisposable<ClassicOfferPopup>(AssetsConstants.CLASSICOFFER_POPUP, Constants.Scenes.UI);
            popup.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService
                );
            return popup;
        }
    }
}
