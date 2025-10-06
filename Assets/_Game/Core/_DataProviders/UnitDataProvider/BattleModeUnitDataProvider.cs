using System.Collections.Generic;
using _Game.Core._DataPresenters._WeaponDataProvider;
using _Game.Core._IconContainer;
using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.Core.Configs.Models._Skills;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Data;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Core.Navigation.Timeline;
using _Game.Core.UserState._State;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.UI._Skills.Scripts;
using UnityEngine;

namespace _Game.Core._DataProviders.UnitDataProvider
{
    public class BattleModeUnitDataProvider : UnitDataProviderBase, IBattleModeUnitDataProvider
    {
        private readonly IBattleNavigator _battleNavigator;
        private readonly IConfigRepository _configRepository;
        private readonly ITimelineNavigator _timelineNavigator;
        private readonly IAgeNavigator _ageNavigator;
        private readonly BoostContainer _boost;
        private readonly IBattleModeWeaponDataProvider _weaponDataProvider;
        private readonly WarriorIconContainer _iconContainer;
        private readonly ISkillService _skillService;
        private readonly IMyLogger _logger;

        private readonly Dictionary<UnitType, IUnitCreationStrategy> _creationStrategies;

        private IDifficultyConfigRepository DifficultyConfig => _configRepository.DifficultyConfigRepository;
        private ITimelineConfigRepository TimelineConfig => _configRepository.TimelineConfigRepository;

        public BattleModeUnitDataProvider(
            IBattleNavigator battleNavigator,
            IConfigRepository configRepositoryRepository,
            ITimelineNavigator timelineNavigator,
            IAgeNavigator ageNavigator,
            BoostContainer boost,
            WarriorIconContainer iconContainer,
            IBattleModeWeaponDataProvider weaponDataProvider,
            ISkillService skillService,
            IMyLogger logger)
        {
            _battleNavigator = battleNavigator;
            _logger = logger;
            _configRepository = configRepositoryRepository;
            _timelineNavigator = timelineNavigator;
            _ageNavigator = ageNavigator;
            _boost = boost;
            _weaponDataProvider = weaponDataProvider;
            _iconContainer = iconContainer;
            _skillService = skillService;
            
            _creationStrategies = new Dictionary<UnitType, IUnitCreationStrategy>
            {
                { UnitType.Ghosts, new GhostsUnitCreationStrategy(this) },
   
                { UnitType.Light, new StandardUnitCreationStrategy(this) },
                { UnitType.Medium, new StandardUnitCreationStrategy(this) },
                { UnitType.Heavy, new StandardUnitCreationStrategy(this) },
            };
        }

        public IUnitData GetDecoratedUnitData(Faction faction, UnitType type, Skin skin)
        {
            if (_creationStrategies.TryGetValue(type, out var strategy))
            {
                return strategy.CreateUnitData(faction, type, skin);
            }

            return new StandardUnitCreationStrategy(this).CreateUnitData(faction, type, skin);
        }
        
        public IEnumerable<IUnitData> GetAllPlayerUnits()
        {
            foreach (var warriorConfig in TimelineConfig.GetRelatedAgeWarriors(_timelineNavigator.CurrentTimelineId, _ageNavigator.CurrentIdx))
            {
                yield return GetPlayerUnitDataFor(warriorConfig.Type, Skin.Ally);
            }
        }

        public override IUnitData GetPlayerUnitDataFor(UnitType type, Skin skin)
        {
            WarriorConfig warriorConfig = TimelineConfig.GetAgeRelatedWarrior(_timelineNavigator.CurrentTimelineId,
                _ageNavigator.CurrentIdx, type);

            IWeaponData weaponData = _weaponDataProvider.GetWeaponData(warriorConfig.WeaponId, Faction.Player);

            var iconReference = warriorConfig.GetIconReferenceFor(skin);

            Sprite icon = _iconContainer.Get(iconReference.Atlas.AssetGUID).Get(iconReference.IconName);

            UnitData vanillaData = new UnitData.UnitDataBuilder()
                .WithHealth(warriorConfig.Health)
                .WithLayer(warriorConfig.GetUnitLayer(Faction.Player))
                .WithAggroLayer(warriorConfig.GetAggroLayer(Faction.Player))
                .WithAttackLayer(warriorConfig.GetAttackLayer(Faction.Player))
                .WithRVOLayer(warriorConfig.GetRVOLayer(Faction.Player))
                .WithAttackPerSecond(warriorConfig.AttackPerSecond)
                .WithNane(warriorConfig.Name)
                .WithSkin(skin)
                .WithType(type)
                .WithAttackDistance(warriorConfig.AttackDistance)
                .WithFoodPrice(warriorConfig.FoodPrice)
                .WithCoinsPerKill(warriorConfig.CoinsPerKill)
                .WithSpeed(warriorConfig.Speed)
                .WithPrice(new[]
                {
                    new CurrencyData
                    {
                        Type = CurrencyType.Coins,
                        Amount = warriorConfig.Price
                    }
                })
                .WithPrefabKey(warriorConfig.GetPrefabKeyFor(skin))
                .WithIcon(icon)
                .WithWeapon(weaponData)
                .WithHealthBoost(warriorConfig.PlayerHealthMultiplier)
                .WithObjectsPositionSettings(warriorConfig.GetPositionSettings(skin))
                .WithIsPushable(warriorConfig.IsPushable)
                .Build();

            HealthBoostDecorator healthBoostOldDecorator = new HealthBoostDecorator(vanillaData,
                _boost.GetBoostValue(BoostSource.Total, BoostType.AllUnitHealth));

            return healthBoostOldDecorator;
        }

        public override IUnitData GetEnemyUnitDataFor(UnitType type, Skin skin)
        {
            var difficulty = DifficultyConfig.GetDifficultyValue(_timelineNavigator.CurrentTimelineNumber);

            WarriorConfig warriorConfig = TimelineConfig.GetBattleRelatedWarrior(_timelineNavigator.CurrentTimelineId,
                _battleNavigator.CurrentBattleIdx, type);

            IWeaponData weaponData = _weaponDataProvider.GetWeaponData(warriorConfig.WeaponId, Faction.Enemy);

            var iconReference = warriorConfig.GetIconReferenceFor(Skin.Hostile);

            Sprite icon = _iconContainer.Get(iconReference.Atlas.AssetGUID).Get(iconReference.IconName);

            UnitData vanillaData = new UnitData.UnitDataBuilder()
                .WithHealth(warriorConfig.Health)
                .WithLayer(warriorConfig.GetUnitLayer(Faction.Enemy))
                .WithAggroLayer(warriorConfig.GetAggroLayer(Faction.Enemy))
                .WithAttackLayer(warriorConfig.GetAttackLayer(Faction.Enemy))
                .WithRVOLayer(warriorConfig.GetRVOLayer(Faction.Enemy))
                .WithAttackPerSecond(warriorConfig.AttackPerSecond)
                .WithNane(warriorConfig.Name)
                .WithSkin(skin)
                .WithType(type)
                .WithAttackDistance(warriorConfig.AttackDistance)
                .WithFoodPrice(warriorConfig.FoodPrice)
                .WithCoinsPerKill(warriorConfig.CoinsPerKill)
                .WithSpeed(warriorConfig.Speed)
                .WithPrice(new[]
                {
                    new CurrencyData
                    {
                        Type = CurrencyType.Coins,
                        Amount = warriorConfig.Price
                    }
                })
                .WithPrefabKey(warriorConfig.GetPrefabKeyFor(skin))
                .WithIcon(icon)
                .WithWeapon(weaponData)
                .WithHealthBoost(warriorConfig.EnemyHealthMultiplier)
                .WithObjectsPositionSettings(warriorConfig.GetPositionSettings(skin))
                .WithIsPushable(warriorConfig.IsPushable)
                .Build();

            UnitLootBoostDecorator lootBoostOldDecorator
                = new UnitLootBoostDecorator(vanillaData,
                    _boost.GetBoostValue(BoostSource.Total, BoostType.CoinsGained));

            HealthBoostDecorator healthBoostOldDecorator = new HealthBoostDecorator(lootBoostOldDecorator,
                difficulty);

            return healthBoostOldDecorator;
        }

        public override IUnitData GetDecoratedGhostsUnitData(Skin skin)
        {
            SkillModel hugsModel = _skillService.GetSkillModel(SkillType.Ghosts);
            GhostsSkillConfig ghostsSkillConfig = hugsModel.GetSpecificConfig<GhostsSkillConfig>();
            
            WarriorConfig warriorConfig = TimelineConfig.GetAgeRelatedWarrior(_timelineNavigator.CurrentTimelineId,
                _ageNavigator.CurrentIdx, UnitType.Light);
            
            IWeaponData weaponData = _weaponDataProvider.GetWeaponData(warriorConfig.WeaponId, Faction.Player);

            UnitData vanillaData = new UnitData.UnitDataBuilder()
                .WithHealth(warriorConfig.Health * warriorConfig.PlayerHealthMultiplier)
                
                .WithLayer(warriorConfig.GetUnitLayer(Faction.Player))
                .WithAggroLayer(warriorConfig.GetAggroLayer(Faction.Player))
                .WithAttackLayer(warriorConfig.GetAttackLayer(Faction.Player))
                .WithRVOLayer(warriorConfig.GetRVOLayer(Faction.Player))
                
                .WithLayer(ghostsSkillConfig.GetUnitLayer())
                .WithAggroLayer(ghostsSkillConfig.GetAggroLayer())
                .WithAttackLayer(ghostsSkillConfig.GetAttackLayer())
                .WithRVOLayer(ghostsSkillConfig.GetRVOLayer())
                
                .WithAttackPerSecond(ghostsSkillConfig.AttackPerSecond)
                .WithSkin(skin)
                
                .WithType(UnitType.Ghosts)
                
                .WithAttackDistance(warriorConfig.DefaultAttackDistance)
                .WithSpeed(ghostsSkillConfig.GhostUnitSpeed)
                .WithPrefabKey(ghostsSkillConfig.GhostPrefabKey)
                
                .WithWeapon(weaponData)
                
                .WithObjectsPositionSettings(warriorConfig.GetPositionSettings(skin))
                .WithIsPushable(true)
                .Build();

            HealthBoostDecorator healthBoostOldDecorator = new HealthBoostDecorator(vanillaData,
                _boost.GetBoostValue(BoostSource.Total, BoostType.AllUnitHealth) * hugsModel.GetSkillValue());

            return healthBoostOldDecorator;

        }
    }
}