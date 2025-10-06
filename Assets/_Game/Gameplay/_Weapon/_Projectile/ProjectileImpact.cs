using System;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Weapon._Projectile
{
    public abstract class ProjectileImpact
    {
        protected ITarget _target;
        protected Action _callback;
        protected Transform _self;
        protected IWeaponData _weaponData;

        public virtual void Initialize(
            Transform self,
            ITarget target,
            IWeaponData weaponData,
            Action callback)
        {
            _self = self;
            _target = target;
            _callback = callback;
            _weaponData = weaponData;
        }

        public abstract void Hit();
    }
}