using System;
using _Game.Gameplay._BattleField.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Weapon._Projectile
{
    public class SplashProjectileImpact : ProjectileImpact
    {
        private readonly ITargetRegistry _targetRegistry;
        
        private readonly Collider2D[] _hitBuffer = new Collider2D[3];

        public SplashProjectileImpact(ITargetRegistry targetRegistry)
        {
            _targetRegistry = targetRegistry;
        }
        
        public override void Hit()
        {
            ApplyDamageAndEffects();
            _callback?.Invoke();
        }
        
        private void ApplyDamageAndEffects()
        {
            int count = Physics2D.OverlapCircleNonAlloc(_self.position, _weaponData.SplashRadius, _hitBuffer, _weaponData.CollisionMask);

            if (count == 0)
            {
                _callback?.Invoke();
                return;
            }

            float[] distances = new float[count];

            for (int i = 0; i < count; i++)
            {
                distances[i] = Vector2.Distance(_self.position, _hitBuffer[i].transform.position);
            }

            Array.Sort(distances, _hitBuffer, 0, count);

            float damageToDeal = _weaponData.Damage;

            for (int i = 0; i < count; i++)
            {
                _targetRegistry.TryGetTarget(_hitBuffer[i], out var target);
                
                if (target is {IsActive: true })
                {
                    damageToDeal = _weaponData.SplashDamageRatio * _weaponData.Damage;
                    if (i == 0)
                        damageToDeal += _weaponData.Damage;
                    target.TakeDamage(damageToDeal);
                }
            }
            
            _callback?.Invoke();
        }
    }
}