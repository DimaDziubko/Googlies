using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.Services.IAP;
using _Game.Core.Services.UserContainer;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.Settings.Scripts
{
    public class SettingsPopupProvider : LocalAssetLoader, ISettingsPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IIAPService _iapService;
        private readonly IUserContainer _userContainer;

        public SettingsPopupProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IIAPService iapService,
            IUserContainer userContainer
            )
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _iapService = iapService;
            _userContainer = userContainer;
        }
        public async UniTask<Disposable<SettingsPopup>> Load()
        {
            var popup = await LoadDisposable<SettingsPopup>(AssetsConstants.SETTINGS, Constants.Scenes.UI);
            popup.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _iapService,
                _userContainer
                );
            return popup;
        }
    }
}