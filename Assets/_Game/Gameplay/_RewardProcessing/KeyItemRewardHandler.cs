using System;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core._Reward;
using _Game.Core.Boosts;

namespace _Game.Gameplay._RewardProcessing
{
    public class KeyItemRewardHandler : IItemRewardHandler
    {
        private readonly IDungeonModelFactory _dungeonsModelFactory;

        public KeyItemRewardHandler(IDungeonModelFactory dungeonModelFactory)
        {
            _dungeonsModelFactory = dungeonModelFactory;
        }
        public void Handle(IRewardItem rewardItem, ItemSource source, Action onComplete = null)
        {
            if (rewardItem.Details is KeyItemLocal keyItem)
            {
                var model = _dungeonsModelFactory.GetModel(keyItem.DungeonType);
                if (model != null)
                {
                    model.AddKeys((int)rewardItem.Amount, source);
                }
            }
        }
    }
}