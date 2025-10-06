using System;
using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Data;
using _Game.Core.UserState._State;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils;

namespace _Game.Core._DataProviders.UnitDataProvider
{
    public class ZombieRushModeBaseDataProvider : IZombieRushModeBaseDataProvider
    {
        private readonly IConfigRepository _config;
        private readonly BoostContainer _boosts;
        private readonly IDungeonModelFactory _factory;
        private readonly IMyLogger _logger;

        private ITimelineConfigRepository TimelineConfig => _config.TimelineConfigRepository;

        public ZombieRushModeBaseDataProvider(
            BoostContainer boosts,
            IDungeonModelFactory factory,
            IMyLogger logger,
            IConfigRepository config)
        {
            _boosts = boosts;
            _logger = logger;
            _config = config;
            _factory = factory;
        }

        public IBaseData GetBaseData(Faction faction)
        {
            switch (faction)
            {
                case Faction.Player:
                    return GetPlayerBaseData();
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            }
        }

        private IBaseData GetPlayerBaseData()
        {
            var ageConfig = TimelineConfig.GetRelatedAge(_factory.GetModel(DungeonType.ZombieRush).TimelineId, 0);
            
            BaseData baseData = new BaseData.BaseDataBuilder()
                .WithAssetReference(ageConfig.BasePrefab)
                .WithLayer(Constants.Layer.PLAYER_BASE)
                .WithHealth(1)
                .Build();

            BaseHealthBoostDecorator healthBoostDecorator
                = new BaseHealthBoostDecorator(baseData,
                    _boosts.GetBoostValue(BoostSource.Total, BoostType.BaseHealth));

            return healthBoostDecorator;
        }
    }
}