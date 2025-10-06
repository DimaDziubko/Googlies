using System;
using _Game.Gameplay._Units.Scripts;
using UnityEngine;

namespace _Game.Gameplay._Weapon._Projectile
{
    public abstract class ProjectileMove
    {
        protected Transform _transform;
        protected ITarget _target;
        protected Vector2 _launchPosition;
        protected float _speed;
        protected float _warp;
        protected Action _onTargetReached;
        private float _defaultSpeed;

        public virtual void Initialize(
            Transform transform,
            ITarget target,
            Vector2 launchPosition, 
            float speed,
            float warp,
            Action onTargetReached)
        {
            _transform = transform;
            _target = target;
            _launchPosition = launchPosition;
            _speed = _defaultSpeed = speed;
            _warp = warp;
            _onTargetReached = onTargetReached;
        }

        public abstract void Move(float deltaTime);

        public void SetSpeedFactor(float factor)
        {
            _speed = factor * _defaultSpeed;  
        }
    }
}