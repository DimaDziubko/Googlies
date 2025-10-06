using _Game.Gameplay._Bases.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils.Extensions;

namespace _Game.Gameplay._BattleField.Scripts
{
    public class EnemyBaseEventHandler : IBaseEventHandler
    {
        private readonly IBattleTriggersManager _battleTriggersManager;
        private readonly ILootCoinSpawner _lootCoinSpawner;
        private readonly IVFXProxy _vfxProxy;

        public EnemyBaseEventHandler(
            IBattleTriggersManager battleTriggersManager,
            ILootCoinSpawner lootCoinSpawner,
            IVFXProxy vfxProxy)
        {
            _battleTriggersManager = battleTriggersManager;
            _lootCoinSpawner = lootCoinSpawner;
            _vfxProxy = vfxProxy;
        }
        public void OnBaseHit(Base @base, float damage, float maxHealth)
        {
            if (SpawnLootCoin(@base, damage, maxHealth)) return;
            
            SpawnDamageText(@base, damage);
        }

        private void SpawnDamageText(Base @base, float damage)
        {
            _vfxProxy.SpawnDamageTextRight(@base.DamageTextPosition, damage.ToCompactFormat(10));
        }

        private bool SpawnLootCoin(Base @base, float damage, float maxHealth)
        {
            float coinsPerBase = @base.CoinsPerBase;

            if (coinsPerBase <= 0) return true;

            float coinsPerHp = @base.CoinsPerBase / maxHealth;

            float coinsToDrop = damage * coinsPerHp;

            if (coinsToDrop > @base.CoinsState)
            {
                coinsToDrop = @base.CoinsState;
            }

            _lootCoinSpawner.SpawnLootCoin(@base.Position, coinsToDrop);

            @base.SpendCoins(coinsToDrop);
            
            return false;
        }

        public void OnBaseDestructionStarted(Base @base) =>
            _battleTriggersManager.BaseDestructionStarted(Faction.Enemy, @base);

        public void OnBaseDestructionCompleted(Base @base) =>
            _battleTriggersManager.BaseDestructionCompleted(Faction.Enemy, @base);
    }
}