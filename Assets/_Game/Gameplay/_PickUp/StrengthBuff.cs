using _Game.Core._Logger;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using DG.Tweening;
using UnityEngine;

namespace _Game.Gameplay._PickUp
{
    public class StrengthBuff : IDebuff
    {
        public DebuffType DebuffType => DebuffType.Strength;

        private UnitBase _unit;
        private readonly StrengthSkillConfig _config;
        private readonly int _level;
        private readonly IMyLogger _logger;

        private IWeaponData _vanillaWeaponData;
        private IUnitData _vanillaUnitData;
        private Vector3 _vanillaScale;
        private Vector3 _weaponVanillaScale;
        private Tween _tween;

        public StrengthBuff(
            StrengthSkillConfig config,
            IMyLogger logger,
            int level)
        {
            _config = config;
            _level = level;
            _logger = logger;
        }

        public void Assign(UnitBase unitBase)
        {
            Cleanup();
            _unit = unitBase;
            ApplyBuffs();
        }

        private void Cleanup()
        {
            if (_tween != null)
            {
                _tween.Kill();
                _tween = null;
            }
        }

        private void ApplyBuffs()
        {
            ApplyHealthBuff();
            ApplyDamageBuff();
            ApplyScale();
        }

        private void ApplyScale()
        {
            _vanillaScale = _unit.Pivot.Scale;
            var targetScale = _config.IncreaseScale;
            _tween =  _unit.Pivot.Transform.DOScale(targetScale, 2f);
            
            _weaponVanillaScale = _unit.Weapon.GetWeaponScale();
            _unit.Weapon.SetWeaponScale(targetScale);
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
            Cleanup();
            RemoveBuffs();
        }

        private void RemoveBuffs()
        {
            RemoveAttackBuff();
            RemoveHealthBuff();
            ResetScale();
        }

        private void ResetScale()
        {
            _unit.Pivot.Scale = _vanillaScale;
            _unit.Weapon.SetWeaponScale(_weaponVanillaScale);
        }

        private void RemoveHealthBuff() => 
            _unit.SetUnitData(_vanillaUnitData);

        private void RemoveAttackBuff() => 
            _unit.Weapon.SetData(_vanillaWeaponData);
    }
}