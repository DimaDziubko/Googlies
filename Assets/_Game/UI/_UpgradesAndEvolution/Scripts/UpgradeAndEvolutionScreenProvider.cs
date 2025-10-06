using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._UpgradesAndEvolution.Scripts
{
    public class UpgradeAndEvolutionScreenProvider : LocalAssetLoader, IUpgradeAndEvolutionScreenProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IUpgradesAndEvolutionScreenPresenter _presenter;
        
        private Disposable<UpgradeAndEvolutionScreen> _screen;
        
        public UpgradeAndEvolutionScreenProvider(
            IWorldCameraService cameraService,
            IUpgradesAndEvolutionScreenPresenter presenter)
        {
            _cameraService = cameraService;
            _presenter = presenter;
        }

        public async UniTask<Disposable<UpgradeAndEvolutionScreen>> Load()
        {
            if (_screen != null) return _screen;
            
            _screen = await LoadDisposable<UpgradeAndEvolutionScreen>(AssetsConstants.UPGRADE_AND_EVOLUTION_SCREEN, Constants.Scenes.UI);

            _screen.Value.Construct(
                _cameraService,
                _presenter);

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