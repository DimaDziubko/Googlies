using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Gameplay.Vfx.Scripts
{
    public class FoodRecover : VfxEntity
    {
        [SerializeField, Required] private Transform _transform;
        [SerializeField, Required] private Canvas _canvas;
        [SerializeField, Required] private CanvasGroup _canvasGroup;
        [SerializeField, Required] private Image _iconHolder;
        [SerializeField, Required] private TMP_Text _label;
        
        [SerializeField] private float _duration = 2f;
        [SerializeField] private float _waitDuration = 1f;
        [SerializeField] private float _fadeDuration = 0.5f;
        
        private float _age;

        private Tween _fadeTween;
        
        private Vector3 Position
        {
            get => _transform.position;
            set => _transform.position = value;
        }
        
        public void Initialize(
            Vector3 position,
            Sprite sprite,
            string value)
        {
            _canvas.worldCamera = _cameraService.MainCamera;
            
            Position = position;
            _iconHolder.sprite = sprite;
            _label.text = value;
            _canvasGroup.alpha = 1;
            _age = 0;
        }

        public void Play()
        {
            _fadeTween?.Kill();
            _fadeTween = DOVirtual.DelayedCall(_waitDuration, () =>
            {
                _canvasGroup.DOFade(0, _fadeDuration)
                    .OnComplete(() => OriginFactory.Reclaim(this));
            });
        }
        
        public override bool GameUpdate(float deltaTime)
        {
            _age += deltaTime;
            if (_age >= _duration)
            {
                OriginFactory.Reclaim(this);
                return false;
            }

            return true;
        }

        public override void Recycle()
        {
            _fadeTween?.Kill(); 
            base.Recycle();
        }
    }
}