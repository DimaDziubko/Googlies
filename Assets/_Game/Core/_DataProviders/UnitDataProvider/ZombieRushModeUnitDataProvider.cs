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
using _Game.Core.UserState._State;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using _Game.UI._Skills.Scripts;
using UnityEngine;

namespace _Game.Core._DataProviders.UnitDataProvider
{
    public class ZombieRushModeUnitDataProvider : UnitDataProviderBase, IZombieRushModeUnitDataProvider
    {
        private readonly IZombieRushModeWeaponDataProvider _weaponDataProvider;
        private readonly WarriorIconContainer _iconContainer;
        private readonly IConfigRepository _config;
        private readonly IDungeonModelFactory _factory;
        private readonly BoostContainer _boosts;
        private readonly ISkillService _skillService;
        private readonly IMyLogger _logger;

        private readonly Dictionary<UnitType, IUnitCreationStrategy> _creationStrategies;
        private ITimelineConfigRepository TimelineConfig => _config.TimelineConfigRepository;

        public ZombieRushModeUnitDataProvider(
            IZombieRushModeWeaponDataProvider weaponDataProvider,
            WarriorIconContainer iconContainer,
            IConfigRepository config,
            IDungeonModelFactory factory,
            BoostContainer boosts,
            ISkillService skillService,
            IMyLogger logger)
        {
            _skillService = skillService;
            _logger = logger;
            _weaponDataProvider = weaponDataProvider;
            _iconContainer = iconContainer;
            _config = config;
            _factory = factory;
            _boosts = boosts;
            
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

        public override IUnitData GetEnemyUnitDataFor(UnitType type, Skin skin)
        {
            WarriorConfig warriorConfig = TimelineConfig.GetBattleRelatedWarrior(_factory.GetModel(DungeonType.ZombieRush).TimelineId,
                (int)type, UnitType.Light);

            IWeaponData weaponData = _weaponDataProvider.GetWeaponData(warriorConfig.WeaponId, Faction.Enemy);

            UnitData vanillaData = new UnitData.UnitDataBuilder()
                .WithHealth(warriorConfig.Health * warriorConfig.EnemyHealthMultiplier)
                .WithLayer(warriorConfig.GetUnitLayer(Faction.Enemy))
                .WithAggroLayer(warriorConfig.GetAggroLayer(Faction.Enemy))
                .WithAttackLayer(warriorConfig.GetAttackLayer(Faction.Enemy))
                .WithRVOLayer(warriorConfig.GetRVOLayer(Faction.Enemy))
                .WithAttackPerSecond(warriorConfig.AttackPerSecond)
                .WithSkin(skin)
                .WithType(type)
                .WithAttackDistance(warriorConfig.AttackDistance)
                .WithSpeed(warriorConfig.Speed)
                .WithPrefabKey(warriorConfig.GetPrefabKeyFor(skin))
                .WithWeapon(weaponData)
                .WithObjectsPositionSettings(warriorConfig.GetPositionSettings(skin))
                .WithIsPushable(warriorConfig.IsPushable)
                .Build();

            HealthBoostDecorator healthBoostOldDecorator = new HealthBoostDecorator(vanillaData,
                _factory.GetModel(DungeonType.ZombieRush).Difficulty);

            return healthBoostOldDecorator;
        }

        public override IUnitData GetDecoratedGhostsUnitData(Skin skin)
        {
            WarriorConfig warriorConfig = TimelineConfig.GetAgeRelatedWarrior(_factory.GetModel(DungeonType.ZombieRush).TimelineId, 0, UnitType.Light);

            IWeaponData weaponData = _weaponDataProvider.GetWeaponData(warriorConfig.WeaponId, Faction.Player);
            
            SkillModel hugsModel = _skillService.GetSkillModel(SkillType.Ghosts);
            GhostsSkillConfig hugsSkillConfig = hugsModel.GetSpecificConfig<GhostsSkillConfig>();
            

            UnitData vanillaData = new UnitData.UnitDataBuilder()
                .WithHealth(warriorConfig.Health * warriorConfig.PlayerHealthMultiplier)
                
                .WithLayer(warriorConfig.GetUnitLayer(Faction.Player))
                .WithAggroLayer(warriorConfig.GetAggroLayer(Faction.Player))
                .WithAttackLayer(warriorConfig.GetAttackLayer(Faction.Player))
                .WithRVOLayer(warriorConfig.GetRVOLayer(Faction.Player))
                
                .WithLayer(hugsSkillConfig.GetUnitLayer())
                .WithAggroLayer(hugsSkillConfig.GetAggroLayer())
                .WithAttackLayer(hugsSkillConfig.GetAttackLayer())
                .WithRVOLayer(hugsSkillConfig.GetRVOLayer())
                
                .WithAttackPerSecond(hugsSkillConfig.AttackPerSecond)
                .WithSkin(skin)
                
                .WithType(UnitType.Ghosts)
                
                .WithAttackDistance(warriorConfig.DefaultAttackDistance)
                .WithSpeed(hugsSkillConfig.GhostUnitSpeed)
                .WithPrefabKey(hugsSkillConfig.GhostPrefabKey)
                
                .WithWeapon(weaponData)
                
                .WithObjectsPositionSettings(warriorConfig.GetPositionSettings(skin))
                .WithIsPushable(warriorConfig.IsPushable)
                
                .Build();

            HealthBoostDecorator healthBoostOldDecorator = new HealthBoostDecorator(vanillaData,
                _boosts.GetBoostValue(BoostSource.Total, BoostType.AllUnitHealth) * hugsModel.GetSkillValue());

            return healthBoostOldDecorator;
        }

        public override IUnitData GetPlayerUnitDataFor(UnitType type, Skin skin)
        {
            WarriorConfig warriorConfig = TimelineConfig.GetAgeRelatedWarrior(_factory.GetModel(DungeonType.ZombieRush).TimelineId, 0, type);

            IWeaponData weaponData = _weaponDataProvider.GetWeaponData(warriorConfig.WeaponId, Faction.Player);

            var iconReference = warriorConfig.GetIconReferenceFor(Skin.Ally);
            Sprite icon = _iconContainer.Get(iconReference.Atlas.AssetGUID).Get(iconReference.IconName);

            UnitData vanillaData = new UnitData.UnitDataBuilder()
                .WithHealth(warriorConfig.Health * warriorConfig.PlayerHealthMultiplier)
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
                .WithPrefabKey(warriorConfig.GetPrefabKeyFor(skin))
                .WithIcon(icon)
                .WithWeapon(weaponData)
                .WithObjectsPositionSettings(warriorConfig.GetPositionSettings(skin))
                .WithIsPushable(warriorConfig.IsPushable)
                .Build();

            HealthBoostDecorator healthBoostOldDecorator = new HealthBoostDecorator(vanillaData,
                _boosts.GetBoostValue(BoostSource.Total, BoostType.AllUnitHealth));

            return healthBoostOldDecorator;
        }

        public IEnumerable<IUnitData> GetAllPlayerUnits()
        {
            foreach (var warriorConfig in TimelineConfig.GetRelatedAgeWarriors(_factory.GetModel(DungeonType.ZombieRush).TimelineId, 0))
            {
                yield return GetPlayerUnitDataFor(warriorConfig.Type, Skin.Ally);
            }
        }
    }
}