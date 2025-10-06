using _Game.Gameplay._RewardProcessing;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Core._Reward
{
    public interface IRewardItem
    {
        [ShowInInspector, ReadOnly]
        int Id { get;}
        
        [ShowInInspector, ReadOnly]
        float Amount { get; }
        bool IsRewardClaimed { get; }
        
        [PreviewField(75, ObjectFieldAlignment.Left)]
        Sprite Icon { get; }
        bool IsRewardReady { get; }
        ItemLocal Details { get; }
        RewardItem Save { get; }

        void ChangeClaimed(bool isClaimed);
        void SetReady(bool isReady);
        void ChangeReady(bool isReady);
        void RequestClaimDelayed(float delaySeconds);
    }
}