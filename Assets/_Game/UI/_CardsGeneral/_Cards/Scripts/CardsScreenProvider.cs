using System.Threading;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityUtils;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardsScreenProvider : LocalAssetLoader, ICardsScreenProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly ICardsScreenPresenter _cardsScreenPresenter;
        private readonly IMyLogger _logger;

        private Disposable<CardsScreen> _screen;

        public CardsScreenProvider(
            IWorldCameraService cameraService,
            ICardsScreenPresenter cardsScreenPresenter,
            IMyLogger logger)
        {
            _cameraService = cameraService;
            _cardsScreenPresenter = cardsScreenPresenter;
            _logger = logger;
        }

        public async UniTask<Disposable<CardsScreen>> Load()
        {
            if (_screen != null) return _screen;
            
            _screen = await LoadDisposable<CardsScreen>(AssetsConstants.CARDS_SCREEN, Constants.Scenes.UI);
            _screen.Value.Construct(
                _cameraService,
                _cardsScreenPresenter,
                _logger);
            
            return _screen;
        }
        
        public void Dispose()
        {
            if (_screen != null)
            {
                _screen.Dispose();
                _screen = null;
            }
        }
    }
}