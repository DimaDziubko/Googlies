using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Skills.Scripts
{
    public class ImageFadeComponent : MonoBehaviour
    {
        [SerializeField] private Image[] _images;
        [SerializeField] private float _fadeDuration = 0.25f;
        [SerializeField] private float _startAlpha = 0f;
        [SerializeField] private float _endAlpha = 1f;

        [Header("Use for pulse animation")]
        [SerializeField] private int _loops = 1;
        
        private Tween _currentTween;

        [SerializeField] private bool _prepareOnAwake = true;
        
        private void Awake()
        {
            if (!_prepareOnAwake) return;
            
            SetAlpha(_startAlpha);
        }

        public void ResetSelf() => SetAlpha(_startAlpha);

        public void FadeIn(Action onComplete = null)
        {
            _currentTween?.Kill();
            _currentTween = DOTween.To(GetAlpha, SetAlpha, _endAlpha, _fadeDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void FadeOut(Action onComplete = null)
        {
            _currentTween?.Kill();
            _currentTween = DOTween.To(GetAlpha, SetAlpha, _startAlpha, _fadeDuration)
                .SetEase(Ease.InQuad)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void PulseFade(Action onComplete = null)
        {
            _currentTween?.Kill();
            _currentTween = DOTween.To(GetAlpha, SetAlpha, _endAlpha, _fadeDuration)
                .SetEase(Ease.OutQuad)
                .SetLoops(_loops * 2, LoopType.Yoyo)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void KillAnimations()
        {
            _currentTween?.Kill();
            SetAlpha(_startAlpha);
        }

        private float GetAlpha() => _images.Length > 0 ? _images[0].color.a : 0f;

        private void SetAlpha(float alpha)
        {
            foreach (var image in _images)
            {
                if (image != null)
                {
                    Color color = image.color;
                    color.a = alpha;
                    image.color = color;
                }
            }
        }
    }
}