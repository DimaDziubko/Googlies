using System.Collections.Generic;
using _Game.Core._Reward;
using _Game.LiveopsCore.Models.BattlePass;

namespace _Game.UI.BattlePass.Scripts
{
    public class BattlePassClaimPopupPresenter
    {
        private BattlePassEvent _event;

        public BattlePassClaimPopupPresenter(BattlePassEvent @event)
        {
            _event = @event;
        }
        
        public void Claim() =>  _event.RequestClaimUnclaimedRewards();
        public IEnumerable<IRewardItem> GetRewards() => _event.UnclaimedRewards;
        public void SetModel(BattlePassEvent @event) => _event = @event;
    }
}