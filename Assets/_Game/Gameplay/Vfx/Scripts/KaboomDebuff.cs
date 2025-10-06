using _Game.Core.Configs.Models._Skills;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.Utils;
using UnityEngine;

namespace _Game.Gameplay.Vfx.Scripts
{
    public class KaboomDebuff : IDebuff
    {
        public DebuffType DebuffType => DebuffType.Shockwave;

        private UnitBase _unit;
        private readonly KaboomView _view;
        private readonly KaboomSkillConfig _config;
        private readonly int _level;
        private readonly ITargetRegistry _targetRegistry;

        private readonly Collider2D[] _hitBuffer = new Collider2D[5];

        public KaboomDebuff(
            ITargetRegistry targetRegistry,
            KaboomView view,
            KaboomSkillConfig config,
            int level)
        {
            _targetRegistry = targetRegistry;
            _view = view;
            _config = config;
            _level = level;
        }

        public void Assign(UnitBase unitBase)
        {
            _unit = unitBase;
            
            ApplyDamage();
        }

        private void ApplyDamage()
        {
            float damage = _unit.Weapon.GetWeaponData().Damage;
            float damageToDeal = damage * _config.GetValue(_level);

            var collisionMask = (1 << Constants.Layer.MELEE_ENEMY)
                                | (1 << Constants.Layer.RANGE_ENEMY);

            int count = Physics2D.OverlapCircleNonAlloc(
                _unit.SkillEffectParent.position, 
                _config.WaveRadius, 
                _hitBuffer,
                collisionMask);

            for (int i = 0; i < count; i++)
            {
                _targetRegistry.TryGetTarget(_hitBuffer[i], out var target);
                
                if (target is { IsActive: true })
                {
                    target.TakeDamage(damageToDeal);
                }
            }
        }

        public void Remove() => _view.Recycle();
    }
}