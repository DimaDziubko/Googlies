using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Services._Camera;
using _Game.Core.Services.Audio;
using _Game.UI.BattleResultPopup.Scripts;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._BattleResultPopup.Scripts
{
    public class BattleResultPopupProvider : LocalAssetLoader, IBattleResultPopupProvider
    {
        private readonly IWorldCameraService _cameraService;
        private readonly IAudioService _audioService;
        private readonly IMyLogger _logger;
        private readonly BattleResultPopupPresenter _presenter;

        private Disposable<BattleResultPopup> _popup;

        public BattleResultPopupProvider(
            IWorldCameraService cameraService,
            IAudioService audioService,
            IMyLogger logger,
            BattleResultPopupPresenter presenter)
        {
            _cameraService = cameraService;
            _audioService = audioService;
            _presenter = presenter;

            _logger = logger;
        }

        public async UniTask<Disposable<BattleResultPopup>> Load()
        {
            if (_popup != null)
                return _popup;

            _popup = await
                LoadDisposable<BattleResultPopup>(AssetsConstants.BATTLE_RESULT_POPUP, Constants.Scenes.UI);

            _popup.Value.Construct(
                _cameraService,
                _audioService,
                _logger,
                _presenter);

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