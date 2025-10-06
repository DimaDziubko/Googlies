using System;
using _Game.Core.Services._Camera;
using _Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Units.Scripts
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField, Required] private HealthBar _healthBar;
        [SerializeField, Required] private Collider2D _bodyCollider;

        public event Action Death;
        public event Action<float, float> Hit;

        [ShowInInspector, ReadOnly]
        private float _maxHealth;

        [ShowInInspector, ReadOnly]
        private float _currentHealth;

        [ShowInInspector, ReadOnly]
        private bool _isOnScreen;

        public float MaxHealth => _maxHealth;
        public float CurrentHealth => _currentHealth;


        public void Construct(
            float health,
            IWorldCameraService cameraService)
        {
            _healthBar.Construct(cameraService);

            UpdateData(health);
        }

        public void AddHealth(float amount)
        {
            _currentHealth += amount;
        }

        public void ResetHealth()
        {
            UpdateData(_maxHealth);
        }

        public void UpdateData(float health)
        {
            _maxHealth = _currentHealth = health;
            _healthBar.UpdateHealthView(_currentHealth, _maxHealth);
            _bodyCollider.enabled = true;
        }

        [ShowInInspector, ReadOnly]
        public bool IsDead => _currentHealth <= Constants.ComparisonThreshold.EPSILON;

        public bool IsOnScreen => _isOnScreen;

        public void RotateToCamera() => _healthBar.RotateHealthBarToCamera();

        public void HideHealth()
        {
            _healthBar.Hide();
        }

        public void ShowHealth()
        {
            _healthBar.Show();
        }

        public void SetMaxHealth(float maxHealth)
        {
            var factor = maxHealth / _maxHealth;
            _maxHealth = maxHealth;
            _currentHealth *= factor;
        }

        //public void SetCurrentHealth(float currentHealth) => _currentHealth = currentHealth;

        public void GetDamage(float damage)
        {
            if (IsDead) return;

            ShowHealth();

            _currentHealth -= damage;
            
            Hit?.Invoke(damage, _maxHealth);

            if (IsDead)
            {
                _currentHealth = 0;
                Death?.Invoke();

                if (_bodyCollider != null)
                {
                    _bodyCollider.enabled = false;
                }
            }
            
            _healthBar.UpdateHealthView(_currentHealth, _maxHealth);
        }

#if UNITY_EDITOR

        //Helper


        [Button]
        private void ManualInit()
        {
            _healthBar = GetComponentInChildren<HealthBar>();
            _bodyCollider = GetComponent<Collider2D>();
        }
#endif
    }
}