using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityUtils;

namespace _Game.UI._BoostPopup
{
    public class BoostPopupProvider : LocalAssetLoader, IBoostPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IBoostPopupPresenter _presenter;
        private readonly BoostDataPresenter.Factory _factory;

        private Disposable<BoostPopup> _popup;

        public BoostPopupProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IBoostPopupPresenter presenter,
            BoostDataPresenter.Factory factory)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _presenter = presenter;
            _factory = factory;
        }

        public async UniTask<Disposable<BoostPopup>> Load()
        {
            if (_popup != null) return  _popup;
            
            _popup = await LoadDisposable<BoostPopup>(AssetsConstants.BOOST_POPUP, Constants.Scenes.UI);
            
            _popup.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _presenter,
                _factory);
            
            return  _popup;
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