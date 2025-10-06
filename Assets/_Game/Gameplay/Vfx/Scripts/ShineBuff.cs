using _Game.Core._Logger;
using _Game.Core.Configs.Models._Skills;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;

namespace _Game.Gameplay.Vfx.Scripts
{
    public class ShineBuff : IDebuff
    {
        public DebuffType DebuffType => DebuffType.Shine;

        private UnitBase _unit;
        private readonly HornView _view;
        private readonly HornSkillConfig _config;
        private readonly int _level;
        private readonly IMyLogger _logger;

        private IWeaponData _vanillaWeaponData;
        private IUnitData _vanillaUnitData;

        public ShineBuff(
            HornView view,
            HornSkillConfig config,
            IMyLogger logger,
            int level)
        {
            _view = view;
            _config = config;
            _level = level;
            _logger = logger;
        }

        public void Assign(UnitBase unitBase)
        {
            _unit = unitBase;
            
            ApplyBuffs();
        }

        private void ApplyBuffs()
        {
            ApplyHealthBuff();
            ApplyDamageBuff();
        }

        private void ApplyDamageBuff()
        {
            var damageFactor = _config.GetValue(_level) + 1;
            _vanillaWeaponData = _unit.Weapon.GetWeaponData();
            
            WeaponDamageDecorator damageDecorator = new WeaponDamageDecorator(_vanillaWeaponData, damageFactor);
            _unit.Weapon.SetData(damageDecorator);
        }

        private void ApplyHealthBuff()
        {
            var healthFactor = _config.GetValue(_level) + 1;
            
            _vanillaUnitData = _unit.UnitData;
            
            HealthDecorator healthBoostDecorator = new HealthDecorator(_vanillaUnitData, healthFactor);
            _unit.SetUnitData(healthBoostDecorator);
        }

        public void Remove()
        {
            RemoveBuffs();
            _view.Recycle();
        }

        private void RemoveBuffs()
        {
            RemoveAttackBuff();
            RemoveHealthBuff();
        }

        private void RemoveHealthBuff() => 
            _unit.SetUnitData(_vanillaUnitData);

        private void RemoveAttackBuff() => 
            _unit.Weapon.SetData(_vanillaWeaponData);
    }
}