using DG.Tweening;
using UnityEngine;

namespace _Game.Gameplay._UnitBuilder.Scripts
{
    public class UnitBuilderBtnScaleAnimation : MonoBehaviour
    {
        private const float HALF = 0.5f;

        [SerializeField] private RectTransform _transform;
        [Space]
        [SerializeField] private bool _isEnabled = false;
        [SerializeField] private float _animationDuration = 1.5f;
        [SerializeField] private float _targetScale = 1.1f;
        [SerializeField] private float _initialScale = 1;

        private Tween _scaleTween;

        public void DoScaleAnimation()
        {
            if (!_isEnabled) return;
        
            _scaleTween?.Kill();
        
            _scaleTween = _transform.DOScale(_targetScale, _animationDuration * HALF)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => _transform
                    .DOScale(_initialScale, _animationDuration * HALF)
                    .SetEase(Ease.InOutSine));
        }

        private void OnDestroy()
        {
            _scaleTween?.Kill();
        }
    }
}
