using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.BattlePass.Scripts
{
    public class BattlePassClaimPopupProvider : LocalAssetLoader, IBattlePassClaimPopupProvider
    {
        private readonly IAudioService _audioService;
        private readonly BattlePassClaimPopupPresenter _presenter;
        private readonly IMyLogger _logger;

        public BattlePassClaimPopupProvider(
            BattlePassClaimPopupPresenter presenter,
            IAudioService audioService,
            IMyLogger logger)
        {
            _logger = logger;
            _presenter = presenter;
            _audioService = audioService;
        }
        
        public async UniTask<Disposable<BattlePassClaimPopup>> Load()
        {
            var popup = await LoadDisposable<BattlePassClaimPopup>(
                AssetsConstants.BATTLE_PASS_CLAIM_POPUP, Constants.Scenes.UI);
            
            popup?.Value.Construct(
                _presenter,
                _audioService,
                _logger);
            
            return popup;
        }
    }
}