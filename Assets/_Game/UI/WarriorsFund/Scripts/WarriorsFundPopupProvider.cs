using _Game.Core._Logger;
using _Game.Core.AssetManagement;
using _Game.Core.Services.Audio;
using _Game.Utils;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI.WarriorsFund.Scripts
{
    public interface IWarriorsFundPopupProvider
    {
        UniTask<Disposable<WarriorsFundPopup>> Load();
        void Dispose();
    }

    public class WarriorsFundPopupProvider : LocalAssetLoader, IWarriorsFundPopupProvider
    {
        private readonly IAudioService _audioService;
        private readonly IMyLogger _logger;
        private readonly WarriorsFundPopupPresenter _presenter;

        private Disposable<WarriorsFundPopup> _popup;

        public WarriorsFundPopupProvider(
            WarriorsFundPopupPresenter presenter,
            IAudioService audioService,
            IMyLogger logger)
        {
            _presenter = presenter;
            _audioService = audioService;
            _logger = logger;
        }

        public async UniTask<Disposable<WarriorsFundPopup>> Load()
        {
            if (_popup != null) return _popup; 
                
            _popup = await LoadDisposable<WarriorsFundPopup>(AssetsConstants.WARRIORS_FUND_POPUP, Constants.Scenes.UI);
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