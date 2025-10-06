using _Game.Core._Logger;
using _Game.Core.UserState._State;
using _Game.Gameplay._BattleField.Scripts;
using _Game.LiveopsCore.Models.BattlePass;

namespace _Game.LiveopsCore._GameEventCurrencyManagement
{
    public class ZombieRushModeGameEventCurrencyEarnStrategyFactory : IGameEventCurrencyEarnStrategyFactory
    {
        private readonly IMyLogger _logger;
        private readonly IBattleField _battleField;
        private readonly TemporaryCurrencyBank _temporaryBank;
        private readonly IUnitDropChanceCalculator _dropChance;

        public ZombieRushModeGameEventCurrencyEarnStrategyFactory(
            TemporaryCurrencyBank temporaryBank,
            IBattleField battleField,
            IUnitDropChanceCalculator dropChance,
            IMyLogger logger)
        {
            _dropChance = dropChance;
            _temporaryBank = temporaryBank;
            _battleField = battleField;
             _logger = logger;
        }
        public IGameEventCurrencyEarnStrategy GetEarningStrategy(GameEventBase gameEvent)
        {
            return gameEvent switch
            {
                BattlePassEvent battlePassEvent => 
                    new ZombieRushModeBattlePassPointEarnStrategy(battlePassEvent, _temporaryBank, _battleField, _dropChance, _logger),
                _ => null
            };
        }
    }
}