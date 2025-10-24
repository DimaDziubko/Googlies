using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories.Timeline;
using _Game.Core.Data;
using _Game.Core.UserState._State;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;

namespace _Game.Core._DataPresenters._WeaponDataProvider
{
    public class ZombieRushModeWeaponDataProvider : IZombieRushModeWeaponDataProvider
    {
        private readonly BoostContainer _boosts;
        private readonly IMyLogger _logger;

        private readonly IConfigRepository _config;
        private readonly IDungeonModelFactory _factory;
        private ITimelineConfigRepository TimelineConfig => _config.TimelineConfigRepository;

        public ZombieRushModeWeaponDataProvider(
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
        
        public IWeaponData GetWeaponData(int weaponId, Faction faction)
        {
            if (faction == Faction.Player)
            {
                return CreatePlayerWeaponData(weaponId);
            }

            if(faction == Faction.Enemy)
            {
                return CreateEnemyWeaponData(weaponId);
            }

            _logger.LogError("UnitDataPresenter GetUnitData There is no such context");
            return null;
        }

        private IWeaponData CreatePlayerWeaponData(int weaponId)
        {
            var config = TimelineConfig.ForWeapon(weaponId);
            
            var vanillaData = new WeaponData.WeaponDataBuilder()
                .WithDamage(config.Damage)
                .WithDamageBoost(config.PlayerDamageMultiplier)
                .WithProjectileExplosionKey(config.ProjectileExplosionKey)
                .WithCollisionMask(config.GetCollisionMask(Faction.Player))
                .WithMuzzleKey(config.MuzzleKey)
                .WithWeaponId(config.Id)
                .WithLayer(config.GetLayerForFaction(Faction.Player))
                .WithProjectileSpeed(config.ProjectileSpeed)
                .WithSplashRadius(config.SplashRadius)
                .WithWeaponType(config.WeaponType)
                .WithTrajectoryWarpFactor(config.TrajectoryWarpFactor)
                .WithProjectileKey(config.ProjectileKey)
                .WithWeaponName(config.WeaponName)
                .WithSplashDamageRatio(config.SplashDamageRatio)
                .WithProjectileMoveType(config.trajectoryType)
                .WithProjectileImpactType(config.ProjectileImpactType)
                .WithAttackSfx(config.AttackSfx)
                .WithSpecialAttackSfx(config.SpecialAttackSfx)
                .WithProjectileSfx(config.ProjectileSfx)
                .WithWeaponEnableSfx(config.WeaponEnableSfx)
                .WithColor(config.GetRandomWeaponColorFor(Faction.Player))
                .WithAiming(config.IsAiming)
                .WithAimingSettings(config.AimingSettings)
                .WithImpulseStrength(config.ImpulseStrength)
                .Build();
            
            WeaponDamageBoostDecorator decorated = new WeaponDamageBoostDecorator(vanillaData, 
                _boosts.GetBoostValue(BoostSource.Total, BoostType.AllUnitDamage));
            return decorated;
        }

        private IWeaponData CreateEnemyWeaponData(int weaponId)
        {
            var config = TimelineConfig.ForWeapon(weaponId);
            
            var vanillaData = new WeaponData.WeaponDataBuilder()
                .WithDamage(config.Damage)
                .WithDamageBoost(config.EnemyDamageMultiplier)
                .WithProjectileExplosionKey(config.ProjectileExplosionKey)
                .WithCollisionMask(config.GetCollisionMask(Faction.Enemy))
                .WithMuzzleKey(config.MuzzleKey)
                .WithWeaponId(config.Id)
                .WithLayer(config.GetLayerForFaction(Faction.Enemy))
                .WithProjectileSpeed(config.ProjectileSpeed)
                .WithSplashRadius(config.SplashRadius)
                .WithWeaponType(config.WeaponType)
                .WithTrajectoryWarpFactor(config.TrajectoryWarpFactor)
                .WithProjectileKey(config.ProjectileKey)
                .WithWeaponName(config.WeaponName)
                .WithSplashDamageRatio(config.SplashDamageRatio)
                .WithProjectileMoveType(config.trajectoryType)
                .WithProjectileImpactType(config.ProjectileImpactType)
                .WithAttackSfx(config.AttackSfx)
                .WithSpecialAttackSfx(config.SpecialAttackSfx)
                .WithProjectileSfx(config.ProjectileSfx)
                .WithWeaponEnableSfx(config.WeaponEnableSfx)
                .WithColor(config.GetRandomWeaponColorFor(Faction.Enemy))
                .WithAiming(config.IsAiming)
                .WithAimingSettings(config.AimingSettings)
                .WithImpulseStrength(config.ImpulseStrength)
                .Build();
            
            WeaponDamageBoostDecorator decorated = new WeaponDamageBoostDecorator(vanillaData, 
                _factory.GetModel(DungeonType.ZombieRush).Difficulty);
            return decorated;
        }
    }
}