using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._CardsGeneral._Summoning.Scripts
{
    public interface ISummoningPopupProvider
    {
        UniTask<Disposable<SummoningPopup>> Load();
        void Dispose();
    }

    public class SummoningPopupProvider : LocalAssetLoader, ISummoningPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly SummoningPopupPresenter _popupPresenter;
        
        private Disposable<SummoningPopup> _popup;

        public SummoningPopupProvider(
            SummoningPopupPresenter popupPresenter,
            IWorldCameraService cameraService,
            IAudioService audioService)
        {
            _popupPresenter = popupPresenter;
            _cameraService = cameraService;
            _audioService = audioService;
        }
        public async UniTask<Disposable<SummoningPopup>> Load()
        {
            if (_popup != null) return  _popup;
            _popup = await LoadDisposable<SummoningPopup>(AssetsConstants.SUMMONING_POPUP, Constants.Scenes.UI);
            _popup.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _popupPresenter);
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
