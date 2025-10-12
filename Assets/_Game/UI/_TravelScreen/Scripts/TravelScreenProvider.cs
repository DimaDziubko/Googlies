using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories;
using _Game.Core.Services._Camera;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._TravelScreen.Scripts
{
    public class TravelScreenProvider : LocalAssetLoader, ITravelScreenProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly ITravelScreenPresenter _travelScreenPresenter;
        private readonly IConfigRepository _config;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        
        private Disposable<TravelScreen> _screen;

        public TravelScreenProvider(
            IWorldCameraService cameraService,
            ITravelScreenPresenter travelScreenPresenter,
            IFeatureUnlockSystem featureUnlockSystem,
            IConfigRepository config)
        {
            _cameraService = cameraService;
            _travelScreenPresenter = travelScreenPresenter;
            _featureUnlockSystem = featureUnlockSystem;
            _config = config;
        }

        public async UniTask<Disposable<TravelScreen>> Load()
        {
            if (_screen != null) return _screen;

            _screen = await LoadDisposable<TravelScreen>(AssetsConstants.TRAVEL_SCREEN, Constants.Scenes.UI);
            _screen.Value.Construct(
                _cameraService,
                _travelScreenPresenter,
                 _config,
                _featureUnlockSystem);
            return _screen;
        }

        public void Dispose()
        {
            if (_screen != null)
            {
                _screen.Value.Dispose();
                _screen.Dispose();
                _screen = null;
            }
        }
    }
}