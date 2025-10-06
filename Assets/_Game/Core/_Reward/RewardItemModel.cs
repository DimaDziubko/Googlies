using System;
using _Game.Gameplay._RewardProcessing;
using UnityEngine;

namespace _Game.Core._Reward
{
    public class RewardItemModel : IRewardItem
    {
        public int Id => _save.Id;
        public event Action<RewardItemModel, float> DelayedClaimRequested;
        public event Action<RewardItemModel> IsReadyChanged;
        public event Action OnClaimChanged;
        public event Action OnSetClaimed;

        private readonly RewardItem _save;

        public float Amount => _save.Amount;
        public bool IsRewardClaimed => _save.IsRewardClaimed;
        public Sprite Icon { get; private set; }
        public bool IsRewardReady { get; private set; }
        public ItemLocal Details { get;  private set; }

        public RewardItem Save => _save;

        public RewardItemModel(RewardItem save, ItemLocal config, Sprite icon)
        {
            _save = save;
            Details = config;
            Icon = icon;
        }

        public void SetClaimed(bool isClaimed)
        {
            _save.SetClaimed(isClaimed);
            OnSetClaimed?.Invoke();
        }
        
        public void ChangeClaimed(bool isClaimed)
        {
            _save.SetClaimed(isClaimed);
            OnClaimChanged?.Invoke();
        }

        public void SetReady(bool isReady)
        {
            IsRewardReady = isReady;
        }

        public void ChangeReady(bool isReady)
        {
            if (isReady != IsRewardReady)
            {
                IsRewardReady = isReady;
                IsReadyChanged?.Invoke(this);
            }
        }
        
        public void RequestClaimDelayed(float delaySeconds) => 
            DelayedClaimRequested?.Invoke(this, delaySeconds);
    }
}