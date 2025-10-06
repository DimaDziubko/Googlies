using _Game.Core._Logger;
using _Game.Core.Boosts;
using _Game.Core.UserState._State;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.LiveopsCore.Models.BattlePass;

namespace _Game.LiveopsCore._GameEventCurrencyManagement
{
    public class ZombieRushModeBattlePassPointEarnStrategy : IGameEventCurrencyEarnStrategy
    {
        private readonly IMyLogger _logger;
        private readonly IBattleField _battleField;
        private readonly TemporaryCurrencyBank _temporaryBank;
        private readonly BattlePassEvent _event;
        private readonly IUnitDropChanceCalculator _dropChance;

        public  ZombieRushModeBattlePassPointEarnStrategy(
            BattlePassEvent @event, 
            TemporaryCurrencyBank temporaryBank,
            IBattleField battleField,
            IUnitDropChanceCalculator dropChance,
            IMyLogger logger)
        {
            _dropChance = dropChance;
            _event = @event;
            _temporaryBank = temporaryBank;
            _battleField = battleField;
            _logger = logger;
        }
        public void Execute()
        {
            _logger.Log("Executing ZombieRushModeBattlePassPointEarnStrategy", DebugStatus.Info);
            _battleField.EnemyUnitSpawner.UnitDead += OnEnemyDead;
        }

        public void UnExecute()
        {
            _battleField.EnemyUnitSpawner.UnitDead -= OnEnemyDead;
        }

        private void OnEnemyDead(UnitBase unit)
        {
            if(_event.CurrencyDropSettings.ZombieRushModeSettings.TryGetValue(unit.Type, out UnitLootDropSettingsWithDropChance settings))
            {
                if (_dropChance.ShouldDrop(settings))
                {
                    _temporaryBank.Add(new CurrencyData() {Amount = settings.Amount, Type = _event.CurrencyCellType, Source = ItemSource.LeaderPass});
                    _battleField.VFXProxy.SpawnBattlePassLoot(unit.DeathPosition);
                }
            };
        }
    }
}