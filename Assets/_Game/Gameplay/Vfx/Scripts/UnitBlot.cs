using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Gameplay.Vfx.Scripts
{
    public class UnitBlot : VfxEntity
    {
        [SerializeField, Required] private Transform _transform;
        [SerializeField, Required] private Sprite[] _sprites;
        [SerializeField, Required] private SpriteRenderer _spriteRenderer;
        [SerializeField] private float _lifetime = 20f;
        [SerializeField] private float _startAlpha = 76f;
        [SerializeField] private float _maxAlpha = 255f;
        
        [ShowInInspector, ReadOnly]
        private float _remainingLifetime; 
        
        private Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }
        public void Initialize(Vector3 position)
        {
            var index = Random.Range(0, _sprites.Length);
            _spriteRenderer.sprite = _sprites[index];
            
            Position = position;
            _remainingLifetime = _lifetime;
            SetAlpha(_startAlpha / _maxAlpha);
        }

        public override bool GameUpdate(float deltaTime)
        {
            _remainingLifetime -= deltaTime;

            if (_remainingLifetime <= 0)
            {
                SetAlpha(0f);
                Recycle();
                return false;
            }
            float alpha = Mathf.Lerp(_startAlpha / _maxAlpha, 0f, 1 - (_remainingLifetime / _lifetime));
            SetAlpha(alpha);

            return true;
        }
        private void SetAlpha(float alpha)
        {
            Color color = _spriteRenderer.color;
            color.a = alpha;
            _spriteRenderer.color = color;
        }
    }
}