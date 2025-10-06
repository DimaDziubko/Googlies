using System;
using _Game.Core._DataPresenters._UpgradeItem;
using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Data;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Core.Navigation.Timeline;
using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._UpgradesScreen.Scripts;
using _Game.Utils;

namespace _Game.Core._DataProviders._BaseDataProvider
{
    class BattleModeBaseDataProvider : IBattleModeBaseDataProvider
    {
        private readonly IBattleNavigator _battleNavigator;
        private readonly IMyLogger _logger;
        private readonly IAgeNavigator _ageNavigator;
        private readonly IConfigRepository _config;
        private readonly ITimelineNavigator _timelineNavigator;
        private readonly UpgradeItemContainer _upgradeItemContainer;
        private readonly BoostContainer _boosts;
        
        private ITimelineConfigRepository TimelineConfig => _config.TimelineConfigRepository;
        private IDifficultyConfigRepository DifficultyConfig => _config.DifficultyConfigRepository;
        
        public BattleModeBaseDataProvider(
            BoostContainer boosts,
            IBattleNavigator battleNavigator,
            IMyLogger logger,
            IAgeNavigator ageNavigator,
            IConfigRepository config,
            ITimelineNavigator timelineNavigator,
            UpgradeItemContainer upgradeItemContainer)
        {
            _boosts = boosts;
            _battleNavigator = battleNavigator;
            _logger = logger;
            _ageNavigator = ageNavigator;
            _timelineNavigator = timelineNavigator;
            _config = config;
            _upgradeItemContainer = upgradeItemContainer;
        }
        
        public IBaseData GetBaseData(Faction faction)
        {
            switch (faction)
            {
                case Faction.Player:
                    return GetPlayerBaseData();
                case Faction.Enemy:
                    return GetEnemyBaseData();
                default:
                    throw new ArgumentOutOfRangeException(nameof(faction), faction, null);
            }
        }

        private IBaseData GetPlayerBaseData()
        {
            var ageConfig = TimelineConfig.GetRelatedAge(_timelineNavigator.CurrentTimelineId, _ageNavigator.CurrentIdx);
            var health = _upgradeItemContainer.GetItem(UpgradeItemType.BaseHealth).Value;
            
            _logger.Log($"GET PLAYER BASE {ageConfig.BasePrefab.AssetGUID}", DebugStatus.Warning);
            
            BaseData baseData = new BaseData.BaseDataBuilder()
                .WithAssetReference(ageConfig.BasePrefab)
                .WithLayer(Constants.Layer.PLAYER_BASE)
                .WithHealth(health)
                .Build();
            
            BaseHealthBoostDecorator healthBoostDecorator
                = new BaseHealthBoostDecorator(baseData,
                    _boosts.GetBoostValue(BoostSource.Total, BoostType.BaseHealth));
            
            return healthBoostDecorator;
        }

        private IBaseData GetEnemyBaseData()
        {
            var battleConfig = TimelineConfig.GetRelatedBattle(_timelineNavigator.CurrentTimelineId, _battleNavigator.CurrentBattleIdx);

            _logger.Log($"GET ENEMY BASE {battleConfig.BasePrefab.AssetGUID}", DebugStatus.Warning);
            
            BaseData baseData = new BaseData.BaseDataBuilder()
                .WithAssetReference(battleConfig.BasePrefab)
                .WithLayer(Constants.Layer.ENEMY_BASE)
                .WithHealth(battleConfig.EnemyBaseHealth)
                .WithCoins(battleConfig.CoinsPerBase)
                .Build();
            
            BaseLootBoostDecorator lootBoostDecorator
                = new BaseLootBoostDecorator(baseData,
                    _boosts.GetBoostValue(BoostSource.Total,  BoostType.CoinsGained));
            
            BaseHealthBoostDecorator healthBoostDecorator
                = new BaseHealthBoostDecorator(lootBoostDecorator,
                    DifficultyConfig.GetDifficultyValue(_timelineNavigator.CurrentTimelineNumber));
            return healthBoostDecorator;
        }
    }
}