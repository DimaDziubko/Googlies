using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.LiveopsCore.Models.WarriorsFund;
using _Game.UI.WarriorsFund.Scripts;
using _Game.Utils.Disposable;

namespace _Game.LiveopsCore._UnclaimedRewardShowStrategies
{
    public class WarriorsFundUnclaimedRewardsShowStrategy : IGameEventUnclaimedRewardShowStrategy
    {
        private WarriorsFundClaimPopupPresenter _presenter;
        
        private readonly WarriorsFundEvent _model;
        private readonly IAudioService _audioService;
        private readonly IMyLogger _logger;

        private WarriorsFundClaimPopupProvider _popupProvider;
        private Disposable<WarriorsFundClaimPopup> _popup;

        public WarriorsFundUnclaimedRewardsShowStrategy (
            WarriorsFundEvent model,
            IAudioService audioService,
            IMyLogger logger)
        {
            _logger = logger;
            _model = model;
            _audioService = audioService;
        }
       
        public async void Execute()
        {
            if (_presenter == null)
            {
                
                _presenter = new WarriorsFundClaimPopupPresenter(_model);
            }
            else
            {
                _presenter.SetModel(_model);
            }
            
            _popupProvider ??= new WarriorsFundClaimPopupProvider(
                _presenter,
                _audioService,
                _logger);

            _popup = await _popupProvider.Load();
            await _popup.Value.AwaitForExit();
            _popup.Value.Dispose();
            _popup.Dispose();
        }

        public void UnExecute() => _popup?.Dispose();
        public void Cleanup() => _popup?.Dispose();
    }
}