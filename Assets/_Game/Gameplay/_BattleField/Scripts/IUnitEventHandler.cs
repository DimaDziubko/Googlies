using _Game.Gameplay._Units.Scripts;
using _Game.Utils.Extensions;

namespace _Game.Gameplay._BattleField.Scripts
{
    public interface IUnitEventHandler
    {
        void OnUnitHit(UnitBase unit, float damage);
        void OnUnitDead(UnitBase unit);
    }

    public class PlayerUnitEventHandler : IUnitEventHandler
    {
        private readonly IVFXProxy _vfxProxy;

        public PlayerUnitEventHandler(
            IVFXProxy vfxProxy)
        {
            _vfxProxy = vfxProxy;
        }
        public void OnUnitHit(UnitBase unit, float damage) => 
            _vfxProxy.SpawnDamageTextLeft(unit.DamageTextPosition, $"<style=SpecialGrey>{damage.ToCompactFormat(10)}</style>");

        public void OnUnitDead(UnitBase unit)
        {
            
        }
    }
    
    public class EnemyUnitEventHandler : IUnitEventHandler
    {
        private readonly ILootCoinSpawner _lootCoinSpawner;
        private readonly IVFXProxy _vfxProxy;

        public EnemyUnitEventHandler(
            ILootCoinSpawner lootCoinSpawner,
            IVFXProxy vfxProxy)
        {
            _lootCoinSpawner = lootCoinSpawner;
            _vfxProxy = vfxProxy;
        }
        public void OnUnitHit(UnitBase unit, float damage) => 
            _vfxProxy.SpawnDamageTextRight(unit.DamageTextPosition, damage.ToCompactFormat(10));

        public void OnUnitDead(UnitBase unit)
        {
            _lootCoinSpawner.SpawnLootCoin(unit.DeathPosition, unit.CoinsPerKill);
        }
    }
    
    public class ZombieRushEnemyEventHandler : IUnitEventHandler
    {
        private readonly IVFXProxy _vfxProxy;
        private readonly IZombieDeathHandler _zombieDeathHandler;

        public ZombieRushEnemyEventHandler(
            IVFXProxy vfxProxy,
            IZombieDeathHandler zombieDeathHandler)
        {
            _vfxProxy = vfxProxy;
            _zombieDeathHandler = zombieDeathHandler;
        }
        public void OnUnitHit(UnitBase unit, float damage) => 
            _vfxProxy.SpawnDamageTextRight(unit.DamageTextPosition, damage.ToCompactFormat(10));

        public void OnUnitDead(UnitBase unit)
        {
            _zombieDeathHandler.OnZombieDead();
        }
    }
}