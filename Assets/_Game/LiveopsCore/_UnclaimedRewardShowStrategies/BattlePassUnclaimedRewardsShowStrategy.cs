using _Game.Core._Logger;
using _Game.Core.Services.Audio;
using _Game.LiveopsCore.Models.BattlePass;
using _Game.UI.BattlePass.Scripts;
using _Game.Utils.Disposable;

namespace _Game.LiveopsCore._UnclaimedRewardShowStrategies
{
    public class BattlePassUnclaimedRewardsShowStrategy : IGameEventUnclaimedRewardShowStrategy
    {
        private BattlePassClaimPopupPresenter _presenter;
        
       private readonly BattlePassEvent _model;
       private readonly IAudioService _audioService;
       private readonly IMyLogger _logger;

       private BattlePassClaimPopupProvider _popupProvider;
       private Disposable<BattlePassClaimPopup> _popup;

       public BattlePassUnclaimedRewardsShowStrategy(
           BattlePassEvent model,
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
                
                _presenter = new BattlePassClaimPopupPresenter(_model);
            }
            else
            {
                _presenter.SetModel(_model);
            }
            
            _popupProvider ??= new BattlePassClaimPopupProvider(
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