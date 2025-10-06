using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay._Environment
{
    public class WatterSpot : MonoBehaviour
    {
        [SerializeField] private RectTransform _transform;
        
        [ShowInInspector, ReadOnly]
        private float _lifetime;
        
        [ShowInInspector, ReadOnly]
        private float _speed;
        
        [ShowInInspector, ReadOnly]
        private Vector3 _direction;

        private float _currentLifeTime;

        public bool IsActive {get; private set; }

        public Vector3 Position
        {
            get => _transform.localPosition; 
            set => _transform.localPosition = value;
        }

        public float Lifetime => _lifetime;

        public void Tick(float deltaTime)
        {
            if (!IsActive) return;

            DoMove(deltaTime);
            DoScale(deltaTime);
        }

        public void SetDirection(Vector3 direction)
        {
            _direction = direction;
        }

        public void SetSpeed(float speed)
        {
            _speed = speed;
        }

        public void SetSize(float width, float height)
        {
            _transform.sizeDelta = new Vector2(width, height);
        }

        public void SetSetLifetime(float lifetime)
        {
            _lifetime = lifetime;
        }
        
        private void DoMove(float deltaTime)
        {
            _transform.localPosition += _direction * _speed * deltaTime;
        }

        private void DoScale(float deltaTime)
        {
            float scale = Mathf.Sin(Mathf.PI * (_currentLifeTime / _lifetime));
            _transform.localScale = new Vector3(scale, scale, 1);
            
            _currentLifeTime += deltaTime;
            if (_currentLifeTime >= _lifetime)
            {
                IsActive = false;
                _transform.localScale = Vector3.zero;
            }
        }

        public void SetCurrentLifeTime(float currentLifeTime)
        {
            _currentLifeTime = currentLifeTime;
        }

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
        }
    }
}