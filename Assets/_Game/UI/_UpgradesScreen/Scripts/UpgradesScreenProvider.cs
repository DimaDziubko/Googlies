using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._UpgradesScreen.Scripts
{
    public class UpgradesScreenProvider : LocalAssetLoader, IUpgradesScreenProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IUpgradesScreenPresenter _upgradesScreenPresenter;

        private Disposable<UpgradesScreen> _screen;

        public UpgradesScreenProvider(
            IWorldCameraService cameraService,
            IUpgradesScreenPresenter upgradesScreenPresenter)
        {
            _cameraService = cameraService;
            _upgradesScreenPresenter = upgradesScreenPresenter;
        }

        public async UniTask<Disposable<UpgradesScreen>> Load()
        {
            if (_screen != null) return _screen;

            _screen = await LoadDisposable<UpgradesScreen>(AssetsConstants.UPGRADES_SCREEN, Constants.Scenes.UI);
            _screen.Value.Construct(
                _cameraService,
                _upgradesScreenPresenter);
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