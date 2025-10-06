using System.Collections.Generic;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories._Cards;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Configs.Repositories.BattleSpeed;
using _Game.Core.Configs.Repositories.DailyTask;
using _Game.Core.Configs.Repositories.Economy;
using _Game.Core.Configs.Repositories.Shop;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.Configs.Repositories
{
    public class ConfigRepository : IConfigRepository
    {
        private readonly IUserContainer _userContainer;
        public IAdsConfigRepository AdsConfigRepository { get; }
        public ICardsConfigRepository CardsConfigRepository { get; }
        public ITimelineConfigRepository TimelineConfigRepository { get; }
        public IIconConfigRepository IconConfigRepository { get; }
        public IBattleSpeedConfigRepository BattleSpeedConfigRepository { get; }
        public IEconomyConfigRepository EconomyConfigRepository { get; }
        public IDailyTaskConfigRepository DailyTaskConfigRepository { get; }
        public IShopConfigRepository ShopConfigRepository { get; }
        public IDifficultyConfigRepository DifficultyConfigRepository { get; }
        public IFeatureConfigRepository FeatureConfigRepository { get; }
        public ISkillConfigRepository SkillConfigRepository { get; }

        public FBConfigIDTO GetFBConfigIds()
        {
            return new FBConfigIDTO()
            {
                AgesID = _userContainer.GameConfig.AgeConfigs.GetValueOrDefault(1).FBConfigId,
                BattlesID = _userContainer.GameConfig.BattleConfigs.GetValueOrDefault(1).FBConfigId,
                DungeonsID = _userContainer.GameConfig.Dungeons.GetValueOrDefault(1).FBConfigId,
                ExtraConfigID = _userContainer.GameConfig.FeatureSettings.FBConfigId,
                WarriorsID = _userContainer.GameConfig.WarriorConfigs.GetValueOrDefault(1).FBConfigId,
                WeaponsID = _userContainer.GameConfig.WeaponConfigs.GetValueOrDefault(1).FBConfigId,
                SkillsID = _userContainer.GameConfig.SkillConfigs.GetValueOrDefault(1).FBConfigID
            };
        }

        
        public ConfigRepository(
            IUserContainer userContainer,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            AdsConfigRepository = new AdsConfigRepository(userContainer);
            CardsConfigRepository = new CardsConfigRepository(userContainer);
            TimelineConfigRepository = new TimelineConfigRepository(userContainer, logger);
            IconConfigRepository = new IconConfigRepository(userContainer, logger);
            BattleSpeedConfigRepository = new BattleSpeedConfigRepository(userContainer);
            EconomyConfigRepository = new EconomyConfigRepository(userContainer, TimelineConfigRepository);
            DailyTaskConfigRepository = new DailyTaskConfigRepository(userContainer);
            ShopConfigRepository = new ShopConfigRepository(userContainer, logger);
            DifficultyConfigRepository = new DifficultyConfigRepository(userContainer);
            FeatureConfigRepository = new FeatureConfigRepository(userContainer);
            SkillConfigRepository = new SkillConfigRepository(userContainer, logger);
        }
    }
}