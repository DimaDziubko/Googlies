using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.WarriorsFund.Scripts
{
    public class WarriorsFundClaimPopupProvider : LocalAssetLoader, IWarriorsFundClaimPopupProvider
    {
        private readonly IAudioService _audioService;
        private readonly WarriorsFundClaimPopupPresenter _presenter;
        private readonly IMyLogger _logger;

        public WarriorsFundClaimPopupProvider(
            WarriorsFundClaimPopupPresenter presenter,
            IAudioService audioService,
            IMyLogger logger)
        {
            _logger = logger;
            _presenter = presenter;
            _audioService = audioService;
        }
        
        public async UniTask<Disposable<WarriorsFundClaimPopup>> Load()
        {
            var popup = await LoadDisposable<WarriorsFundClaimPopup>(
                AssetsConstants.WARRIORS_FUND_CLAIM_POPUP, Constants.Scenes.UI);
            
            popup?.Value.Construct(
                _presenter,
                _audioService,
                _logger);
            
            return popup;
        }
    }
}