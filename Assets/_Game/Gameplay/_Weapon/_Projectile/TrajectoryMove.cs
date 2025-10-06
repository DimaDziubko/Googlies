using System;
using _Game.Gameplay._Units.Scripts;
using _Game.Gameplay._Weapon.Factory;
using UnityEngine;

namespace _Game.Gameplay._Weapon._Projectile
{
    public class TrajectoryMove : ProjectileMove
    {
        private const float EPSILON = 0.01f;

        private readonly ITrajectory _trajectory;

        private float _elapsedTime;
        private float _duration;
        private Vector2 _staticTargetPosition;
        private bool _targetReached;

        public TrajectoryMove(ITrajectory trajectory)
        {
            _trajectory = trajectory;
        }

        public override void Initialize(
            Transform transform,
            ITarget target,
            Vector2 launchPosition,
            float speed,
            float warp,
            Action onTargetReached)
        {
            base.Initialize(transform, target, launchPosition, speed, warp, onTargetReached);

            _elapsedTime = 0f;
            _targetReached = false;

            _staticTargetPosition = target?.Transform.position ?? launchPosition;

            float initialDistance = Vector2.Distance(_launchPosition, _staticTargetPosition);
            _duration = initialDistance > EPSILON ? initialDistance / _speed : 0.01f;
        }

        public override void Move(float deltaTime)
        {
            if (_transform == null || _targetReached)
                return;

            bool hasLiveTarget = _target is { IsActive: true } && !_target.IsDead();

            Vector2 currentTargetPos = hasLiveTarget ? _target.Transform.position : _staticTargetPosition;
            if (hasLiveTarget)
                _staticTargetPosition = currentTargetPos;

            _elapsedTime += deltaTime;
            float t = Mathf.Clamp01(_elapsedTime / _duration);
            
            float distance = Vector2.Distance(_launchPosition, currentTargetPos);
            float adjustedWarp = _warp * distance;

            Vector3 newPosition = _trajectory.GetPosition(_launchPosition, currentTargetPos, t, adjustedWarp);
            
            _transform.position = newPosition;
            
            Vector2 direction = _trajectory.GetDirection(_launchPosition, currentTargetPos, t, adjustedWarp);
            if (direction.sqrMagnitude > 0.001f)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                _transform.rotation = Quaternion.Euler(0f, 0f, angle);
            }

            if (t >= 1f)
            {
                _targetReached = true;
                _onTargetReached?.Invoke();
            }
        }
    }
}