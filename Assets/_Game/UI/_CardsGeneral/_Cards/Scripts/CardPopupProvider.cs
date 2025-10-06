using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Data;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityUtils;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public interface ICardPopupProvider
    {
        UniTask<Disposable<CardPopup>> Load();
        void Dispose();
    }

    public class CardPopupProvider : LocalAssetLoader, ICardPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IIconConfigRepository _config;
        private readonly BoostContainer _boosts;
        private readonly IMyLogger _logger;

        private readonly CardPopupPresenter _cardPopupPresenter;

        private Disposable<CardPopup> _popup;

        public CardPopupProvider(
            CardPopupPresenter cardPopupPresenter,
            IWorldCameraService cameraService,
            IAudioService audioService,
            IIconConfigRepository config,
            BoostContainer boosts,
            IMyLogger logger
            )
        {
            _cardPopupPresenter = cardPopupPresenter;
            _cameraService = cameraService;
            _audioService = audioService;
            _config = config;
            _boosts = boosts;
            _logger = logger;
        }
        
        public async UniTask<Disposable<CardPopup>> Load()
        {
            if (_popup != null) return _popup;
            
            _popup = await LoadDisposable<CardPopup>(AssetsConstants.CARD_POPUP, Constants.Scenes.UI);
            _popup.Value.Construct(
                _cardPopupPresenter, 
                _cameraService,
                _audioService,
                _config,
                _boosts,
                _logger
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