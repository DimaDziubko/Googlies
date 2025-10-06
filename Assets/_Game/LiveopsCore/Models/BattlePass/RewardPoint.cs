using System;
using _Game.Core._Reward;
using _Game.Gameplay._RewardProcessing;
using UnityEngine;

namespace _Game.LiveopsCore.Models.BattlePass
{
    public class RewardPoint : IRewardItem
    {
        public event Action OnChangeLocked;
        public event Action OnChangeReady;
        public event Action<RewardPoint, float> OnClaimRequestedDelayed;
        public event Action OnChangeClaimed;

        private readonly RewardItemModel _reward;
        
        private IAttentionNotifier _notifier;

        public bool IsLocked { get; private set; }
        
        public Sprite Icon => _reward.Icon;

        public ItemLocal Details => _reward.Details;

        public RewardItem Save => _reward.Save;

        public RewardPoint(RewardItemModel reward)
        { 
            _reward = reward;
        }

        public int Id => _reward.Id;

        public float Amount => _reward.Amount;
        public bool IsRewardClaimed => _reward.IsRewardClaimed;
        public bool IsRewardReady => _reward.IsRewardReady;


        public void SetReady(bool isReady) => 
            _reward.SetReady(isReady);

        public void SetLocked(bool isLocked) => 
            IsLocked = isLocked;

        public void ChangeReady(bool isReady)
        {
            if (isReady == IsRewardReady) return;
            _reward.ChangeReady(isReady);
            _notifier?.Notify();
            OnChangeReady?.Invoke();
        }

        public void RequestClaimDelayed(float delaySeconds) => 
            OnClaimRequestedDelayed?.Invoke(this, delaySeconds);

        public void ChangeLocked(bool isLocked)
        {
            if(IsLocked == isLocked) return;
            IsLocked = isLocked;
            _notifier?.Notify();
            OnChangeLocked?.Invoke();
        }

        public void ChangeClaimed(bool isClaimed)
        {
            if(IsRewardClaimed == isClaimed) return;
            _reward.ChangeClaimed(isClaimed);
            _notifier?.Notify();
            OnChangeClaimed?.Invoke();
        }

        public void SetNotifier(IAttentionNotifier notifier)
        {
            _notifier = notifier;
        }
    }
}