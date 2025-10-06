using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.UI.Factory;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._Shop.Scripts._ShopScr
{
    public class ShopProvider : LocalAssetLoader, IShopProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IUIFactory _uiFactory;
        private readonly IShopPresenter _shopPresenter;
        private readonly IMyLogger _logger;

        private Disposable<Shop> _screen;
        
        public ShopProvider(
            IWorldCameraService cameraService,
            IUIFactory uiFactory,
            IShopPresenter shopPresenter,
            IMyLogger logger)
        {
            _cameraService = cameraService;
            _uiFactory = uiFactory;
            _shopPresenter = shopPresenter;
            _logger = logger;
        }
        public async UniTask<Disposable<Shop>> Load()
        {
            if (_screen != null) return _screen;
            _screen = await LoadDisposable<Shop>(AssetsConstants.SHOP, Constants.Scenes.UI);
            _screen.Value.Construct(
                _cameraService.UICameraOverlay,
                _uiFactory,
                _shopPresenter,
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