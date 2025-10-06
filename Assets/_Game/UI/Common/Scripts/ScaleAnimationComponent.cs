using System;
using DG.Tweening;
using UnityEngine;

namespace _Game.UI.Common.Scripts
{
    public class ScaleAnimationComponent : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private float _scaleDuration = 0.25f;
        [SerializeField] private Vector3 _startScale = Vector3.zero;
        [SerializeField] private Vector3 _endScale = Vector3.one;

        [Header("Use for pulse animation")]
        [SerializeField] private int _loops = 1;

        private Tween _currentTween;

        [SerializeField] private bool _prepareOnAwake = true;
        
        private void Awake()
        {
            if(!_prepareOnAwake) return;
            
            _rectTransform.localScale = _startScale;
        }

        public void ResetSelf() => _rectTransform.localScale = _startScale;

        public void ScaleIn(Action onComplete = null)
        {
            _currentTween?.Kill();
            _currentTween = _rectTransform.DOScale(_endScale, _scaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public void ScaleOut(Action onComplete = null)
        {
            _currentTween?.Kill();
            _currentTween = _rectTransform.DOScale(_startScale, _scaleDuration).SetEase(Ease.InBack).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
        
        public void PulseAnimation(Action onComplete = null)
        {
            _currentTween?.Kill();
            _currentTween = _rectTransform.DOScale(_endScale, _scaleDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(_loops * 2, LoopType.Yoyo)
                .OnComplete(() => onComplete?.Invoke());
        }

        public void StopPulse()
        {
            _currentTween?.Kill();
            _rectTransform.localScale = _startScale;
        }

        public void KillAnimations()
        {
            _currentTween?.Kill();
        }
    }
}