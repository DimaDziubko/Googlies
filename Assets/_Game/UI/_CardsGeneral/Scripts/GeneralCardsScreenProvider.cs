using System.Threading;
using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._CardsGeneral.Scripts
{
    public class GeneralCardsScreenProvider : LocalAssetLoader, IGeneralCardsScreenProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IGeneralCardsScreenPresenter _presenter;
        
        private Disposable<GeneralCardsScreen> _screen;
        
        public GeneralCardsScreenProvider(
            IWorldCameraService cameraService,
            IGeneralCardsScreenPresenter presenter)
        {
            _cameraService = cameraService;
            _presenter = presenter;
        }

        public async UniTask<Disposable<GeneralCardsScreen>> Load()
        {
            if (_screen != null) return _screen;
            
            _screen = await LoadDisposable<GeneralCardsScreen>(AssetsConstants.GENERAL_CARDS_SCREEN, Constants.Scenes.UI);
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