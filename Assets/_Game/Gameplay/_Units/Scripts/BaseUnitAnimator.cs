using System;
using _Game.Core._Logger;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public abstract class BaseUnitAnimator : MonoBehaviour
    {
        protected IMyLogger _logger;

        public void Initialize(IMyLogger logger)
        {
            _logger = logger;
            OnInitialize();
        }

        protected abstract void OnInitialize();
        public abstract void SetAttackPerSecond(float attackPerSecond);
        public abstract void GameUpdate(float deltaTime);
        public abstract void LateGameUpdate(float deltaTime);
        public abstract void ResetPose();

        public abstract void PlayIdle();
        public abstract void PlayWalk();
        public abstract void PlayAttack();
        public abstract void TryPlayAggro(Action onComplete = null);
        public abstract void TryStartAiming();
        public abstract void TryStopAiming();
        public abstract void SetTarget(Transform targetTransform);
        public abstract void StopAttack();
        public abstract void PlayDeath(Action onComplete = null);

        public abstract void SetSpeedFactor(float speedFactor);
        public abstract void ResetSpeed();
        public abstract void SetPaused(bool isPaused);
        public abstract void TryCalmDown();

        public abstract void CleanUp();
        public abstract void SetTangent(Vector3 tangent);
        public abstract void PlayBottomLocomotion(float currentSpeed);
        public abstract void PlayTopLocomotion(float currentSpeed);
        public abstract void TryPlayAggroLocomotion(float currentSpeed);
        public abstract void UpdateLocomotionSpeed(float currentSpeed);
    }
}