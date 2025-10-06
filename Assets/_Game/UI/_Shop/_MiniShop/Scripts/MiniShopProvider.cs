using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Core.UserState._State;
using _Game.Temp;
using _Game.UI.Factory;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using Zenject;

namespace _Game.UI._Shop._MiniShop.Scripts
{
    public class MiniShopProvider :
        LocalAssetLoader,
        IMiniShopProvider,
        IInitializable,
        IDisposable
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IUIFactory _uiFactory;
        private readonly IMiniShopPresenter _miniShopPresenter;
        private readonly IMyLogger _logger;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly CurrencyBank _bank;

        public bool IsUnlocked => _featureUnlockSystem.IsFeatureUnlocked(Feature.GemsShopping);

        private Disposable<MiniShop> _miniShop;

        public MiniShopProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IUIFactory uiFactory,
            IMiniShopPresenter miniShopPresenter,
            IMyLogger logger,
            IFeatureUnlockSystem featureUnlockSystem,
            CurrencyBank bank)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _uiFactory = uiFactory;
            _miniShopPresenter = miniShopPresenter;
            _logger = logger;
            _featureUnlockSystem = featureUnlockSystem;
            _bank = bank; 
        }

        public async UniTask<Disposable<MiniShop>> Load()
        {
            _miniShop = await LoadDisposable<MiniShop>(AssetsConstants.MINI_SHOP, Constants.Scenes.UI);

            _miniShop.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _uiFactory,
                _miniShopPresenter,
                _logger,
                _bank);
            return _miniShop;
        }

        void IInitializable.Initialize() =>
            GlobalEvents.OnInsufficientFunds += OnInsufficientFunds;

        void IDisposable.Dispose()
        {
            GlobalEvents.OnInsufficientFunds -= OnInsufficientFunds;
            _miniShop?.Dispose();
        }

        private void OnInsufficientFunds() => _miniShop?.Value.ForceHide();

        public void Dispose()
        {
            if (_miniShop != null)
            {
                _miniShop.Dispose();
                _miniShop = null;
            }
        }
    }
}