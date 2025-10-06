using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.UI.Factory;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public class CardAppearanceScreenProvider : LocalAssetLoader, ICardAppearanceScreenProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IUIFactory _uiFactory;
        private readonly CardAppearancePopupSettings _settings;
        private readonly IConfigRepository _config;
        private Disposable<CardAppearancePopup> _popup;

        public CardAppearanceScreenProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IUIFactory uiFactory,
            CardAppearancePopupSettings settings,
            IConfigRepository config
            )
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _uiFactory = uiFactory;
            _settings = settings;
            _config = config;
        }

        public async UniTask<Disposable<CardAppearancePopup>> Load()
        {
            if (_popup != null) return _popup;

            _popup = await LoadDisposable<CardAppearancePopup>(AssetsConstants.CARD_APPEARANCE_POPUP, Constants.Scenes.UI);
            _popup.Value.Construct(
                _cameraService.UICameraOverlay,
                _audioService,
                _uiFactory,
                _settings,
                _config.IconConfigRepository
                );

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