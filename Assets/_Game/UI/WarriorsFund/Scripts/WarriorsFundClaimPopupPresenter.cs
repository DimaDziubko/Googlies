using System.Collections.Generic;
using _Game.Core._Reward;
using _Game.LiveopsCore.Models.WarriorsFund;

namespace _Game.UI.WarriorsFund.Scripts
{
    public class WarriorsFundClaimPopupPresenter
    {
        private WarriorsFundEvent _event;

        public WarriorsFundClaimPopupPresenter(WarriorsFundEvent @event)
        {
            _event = @event;
        }
        
        public void Claim() =>  _event.RequestClaimUnclaimedRewards();
        public IEnumerable<IRewardItem> GetRewards() => _event.UnclaimedRewards;
        public void SetModel(WarriorsFundEvent @event) => _event = @event;
    }
}