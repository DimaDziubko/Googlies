using System;
using _Game.Core._Reward;
using _Game.Core.Boosts;

namespace _Game.Gameplay._RewardProcessing
{
    public interface IItemRewardHandler
    {
        void Handle(IRewardItem rewardItem, ItemSource source = ItemSource.None, Action onComplete = null);
    }
}