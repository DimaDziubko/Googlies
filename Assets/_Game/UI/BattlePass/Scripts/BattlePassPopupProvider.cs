using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.BattlePass.Scripts
{
    public interface IBattlePassPopupProvider
    {
        UniTask<Disposable<BattlePassPopup>> Load();
        void Dispose();
    }

    public class BattlePassPopupProvider : LocalAssetLoader, IBattlePassPopupProvider
    {
        private readonly IAudioService _audioService;
        private readonly IMyLogger _logger;
        private readonly BattlePassPopupPresenter _presenter;

        private Disposable<BattlePassPopup> _popup;

        public BattlePassPopupProvider(
            BattlePassPopupPresenter presenter,
            IAudioService audioService,
            IMyLogger logger)
        {
            _presenter = presenter;
            _audioService = audioService;
            _logger = logger;
        }

        public async UniTask<Disposable<BattlePassPopup>> Load()
        {
            if (_popup != null) return _popup; 
                
            _popup = await LoadDisposable<BattlePassPopup>(AssetsConstants.BATTLE_PASS_POPUP, Constants.Scenes.UI);
            _popup?.Value.Construct(
                _presenter,
                _audioService,
                _logger);
            
            return _popup;
        }

        public void Dispose()
        {
            if (_popup != null)
            {
                _popup.Value.Dispose();
                _popup.Dispose();
                _popup = null;
            }
        }
    }
}