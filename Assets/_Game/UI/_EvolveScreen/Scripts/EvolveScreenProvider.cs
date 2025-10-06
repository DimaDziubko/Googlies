using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories;
using _Game.Core.Services._Camera;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._EvolveScreen.Scripts
{
    public class EvolveScreenProvider : LocalAssetLoader, IEvolveScreenProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IEvolveScreenPresenter _evolveScreenPresenter;
        private readonly IConfigRepository _config;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;

        private Disposable<EvolveScreen> _screen;

        public EvolveScreenProvider(
            IWorldCameraService cameraService,
            IEvolveScreenPresenter evolveScreenPresenter, 
            IConfigRepository config,
            IFeatureUnlockSystem featureUnlockSystem)
        {
            _cameraService = cameraService;
            _evolveScreenPresenter = evolveScreenPresenter;
            _config = config;
            _featureUnlockSystem = featureUnlockSystem;
        }

        public async UniTask<Disposable<EvolveScreen>> Load()
        {
            if (_screen != null) return _screen;

            _screen = await LoadDisposable<EvolveScreen>(AssetsConstants.EVOLVE_SCREEN, Constants.Scenes.UI);
            _screen.Value.Construct(
                _cameraService,
                _evolveScreenPresenter,
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