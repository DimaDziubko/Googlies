using _Game.Core._Logger;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;

namespace _Game.Gameplay._PickUp
{
    public class StrengthPowerUp :  IVisitor
    {
        private readonly StrengthSkillConfig _config;
        private readonly int _level;
        private readonly IMyLogger _logger;

        public StrengthPowerUp(
            StrengthSkillConfig config,
            IMyLogger logger,
            int level)
        {
            _config = config;
            _level = level;
            _logger = logger;
        }
        public void Visit(UnitBase unit)
        {
            var buff = new StrengthBuff(_config, _logger, _level);
            unit.DebuffMediator.AddOrReplace(buff);
        }
    }
}