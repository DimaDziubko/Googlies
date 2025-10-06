using System.Collections.Generic;
using System.Linq;
using _Game.Core._Reward;
using _Game.Core.AssetManagement;
using _Game.Core.Configs.Models._AgeConfig;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.UserState._State;
using _Game.Gameplay._ItemProvider;
using _Game.Gameplay._RewardProcessing;
using _Game.LiveopsCore.Models.BattlePass;
using _Game.LiveopsCore.Models.WarriorsFund;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Gameplay._RewardItemResolver
{
    public class RewardItemResolver
    {
        private readonly IItemProvider _provider;
        private readonly IAssetRegistry _assetRegistry;
        private readonly IConfigRepository _config;
        private IIconConfigRepository IconConfig => _config.IconConfigRepository;
        private ITimelineConfigRepository TimelineConfigRepository => _config.TimelineConfigRepository;

        public RewardItemResolver(
            IItemProvider provider,
            IConfigRepository config,
            IAssetRegistry assetRegistry)
        {
            _config = config;
            _provider = provider;
            _assetRegistry = assetRegistry;
        }

        public IReadOnlyList<RewardItemModelWithObjective> ResolveRewards(List<RewardItemWithObjective> rewards)
        {
            List<RewardItemModelWithObjective> rewardModels = new List<RewardItemModelWithObjective>();
            foreach (var save in rewards)
            {
                rewardModels.Add(ResolveReward(save));
            }
            return rewardModels;
        }

        public RewardItemModelList ResolveRewardsCollection(RewardItemCollection saveCycleRewardsCollection)
        {
            var rewardItemModelList = new RewardItemModelList()
            {
                Id = saveCycleRewardsCollection.Id,
                RewardItems = ResolveRewards(saveCycleRewardsCollection.CycleRewards)
            };

            return rewardItemModelList;
        }

        public List<RewardItemModel> ResolveRewards(List<RewardItem> rewards)
        {
            List<RewardItemModel> rewardModels = new List<RewardItemModel>();
            foreach (var save in rewards)
            {
                rewardModels.Add(ResolveReward(save));
            }
            return rewardModels;
        }

        public List<IRewardItem> Resolve(List<RewardItem> rewards)
        {
            List<IRewardItem> rewardModels = new List<IRewardItem>();
            foreach (var save in rewards)
            {
                rewardModels.Add(ResolveReward(save));
            }
            return rewardModels;
        }

        private RewardItemModelWithObjective ResolveReward(RewardItemWithObjective reward)
        {
            return new RewardItemModelWithObjective
            {
                RewardItem = ResolveReward(reward.RewardItem),
                Objective = reward.Objective
            };
        }

        public RewardItemModel ResolveReward(RewardItem reward)
        {
            ItemLocal config = _provider.GetItem(reward.Id);
            return new RewardItemModel(reward, config, IconConfig.GetItemIconFor(reward.Id));
        }

        public List<BattlePassPointGroup> Resolve(List<BattlePassPointRewardSavegame> points)
        {
            List<BattlePassPointGroup> groups = new List<BattlePassPointGroup>(points.Count);
            foreach (var point in points)
            {
                groups.Add(Resolve(point));
            }
            return groups;
        }

        private BattlePassPointGroup Resolve(BattlePassPointRewardSavegame point) =>
            new(point.Objective, Resolve(point.FreePoint), Resolve(point.PremiumPoint));

        public async UniTask<List<WarriorsFundPointGroup>> ResolveAsync(List<WarriorsFundPointRewardSavegame> points)
        {
            await _assetRegistry.WarmUp<IList<Sprite>>(IconConfig.GetWarriorsFundAtlasReference());
            IList<Sprite> atlas = await _assetRegistry.LoadAsset<IList<Sprite>>(IconConfig.GetWarriorsFundAtlasReference());
            
            Dictionary<string, Sprite> icons = atlas.ToDictionary(x => x.name);
            
            List<WarriorsFundPointGroup> groups = new List<WarriorsFundPointGroup>(points.Count);
            
            foreach (var point in points)
            {
                groups.Add(Resolve(point, icons));
            }
            
            return groups;
        }

        private WarriorsFundPointGroup Resolve(WarriorsFundPointRewardSavegame point, Dictionary<string, Sprite> icons)
        {
            AgeConfig ageConfig = TimelineConfigRepository.GetRelatedAge(point.Objective.TimelineNumber - 1, point.Objective.AgeNumber - 1);

            Sprite notReadyIcon = icons.GetValueOrDefault("0");
            Sprite readyIcon = icons.ContainsKey(ageConfig.Id.ToString()) ? icons[ageConfig.Id.ToString()] : notReadyIcon;

            return new(point.Objective, notReadyIcon, readyIcon, Resolve(point.FreePoint), Resolve(point.PremiumPoint));
        }

        private RewardPoint Resolve(RewardItem item) => new(ResolveReward(item));
    }
}