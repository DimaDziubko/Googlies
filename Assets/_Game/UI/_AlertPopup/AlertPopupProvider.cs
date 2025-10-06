using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Utils;
using _Game.Utils.Disposable;
using _Game.Utils.Popups;
using Cysharp.Threading.Tasks;

namespace _Game.UI._AlertPopup
{
    public class AlertPopupProvider : LocalAssetLoader, IAlertPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;

        private Disposable<AlertPopup> _popup;
        
        public AlertPopupProvider(
            IWorldCameraService cameraService,
            IAudioService audioService)
        {
            _cameraService = cameraService;
            _audioService = audioService;
        }
        public async UniTask<Disposable<AlertPopup>> Load()
        {
            if (_popup != null) return _popup; 
                
            _popup = await LoadDisposable<AlertPopup>(AssetsConstants.ALERT_POPUP, Constants.Scenes.UI);
            _popup?.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService);
            
            return _popup;
        }
        
        public void Dispose()
        {
            if (_popup != null)
            {
                _popup.Dispose();
                _popup = null;
            }
        }
    }
}