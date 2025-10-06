using System;
using DG.Tweening;
using UnityEngine;

namespace _Game.UI.Common.Scripts
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeComponent : MonoBehaviour
    {
        [SerializeField] private float _fadeDuration = 0.25f;
        private CanvasGroup _canvasGroup;
        private Tween _currentTween;
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0;
        }

        public void FadeIn(Action onComplete = null)
        {
            _currentTween?.Kill();
            _currentTween = _canvasGroup.DOFade(1f, _fadeDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public void FadeOut(Action onComplete = null)
        {
            _currentTween?.Kill();
            _currentTween = _canvasGroup.DOFade(0f, _fadeDuration).SetEase(Ease.InOutQuad).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public void KillAnimations()
        {
            _currentTween?.Kill();
        }
    }
}
