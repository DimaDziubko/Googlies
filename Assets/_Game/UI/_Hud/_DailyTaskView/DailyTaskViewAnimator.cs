using System;
using DG.Tweening;
using UnityEngine;

namespace _Game.UI._Hud._DailyTaskView
{
    public class DailyTaskViewAnimator : MonoBehaviour
    {
        [SerializeField] private RectTransform _viewTransform;
        [SerializeField] private float _normalScale = 1;
        [SerializeField] private float _maxScale = 1.2f;
        [SerializeField] private float _notificationDelay = 10f;
        [SerializeField] private float _notificationTime = 1f;

        [SerializeField] private float _refreshTime = 0.5f;
        
        private Tween _notificationTween;
        private Tween _scaleTween;
        
        public void PlayRefreshAnimation(Action callback)
        {
            if (_scaleTween != null)
            {
                _scaleTween.Kill();
                _scaleTween = null;
            }
            
            _scaleTween = _viewTransform.DOScale(0, _refreshTime / 2).OnComplete(() =>
            {
                callback?.Invoke();
                _viewTransform.DOScale(_normalScale, _refreshTime / 2);
            });
        }

        public void PlayNotificationAnimation()
        {
            if (_notificationTween != null)
            {
                _notificationTween.Kill();
                _notificationTween = null;
            }
            
            _notificationTween = DOTween.Sequence()
                .Append(_viewTransform.DOScale(_maxScale, _notificationTime / 6))
                .Append(_viewTransform.DOScale(_normalScale, _notificationTime / 6))
                .Append(_viewTransform.DOScale(_maxScale, _notificationTime / 6))
                .Append(_viewTransform.DOScale(_normalScale, _notificationTime / 6))
                .Append(_viewTransform.DOScale(_maxScale, _notificationTime / 6))
                .Append(_viewTransform.DOScale(_normalScale, _notificationTime / 6))
                .AppendInterval(_notificationDelay)
                .SetLoops(-1, LoopType.Restart);
        }

        public void StopNotificationAnimation()
        {
            if (_notificationTween != null)
            {
                _notificationTween.Kill();
                _notificationTween = null;
            }

            _viewTransform.localScale = Vector3.one * _normalScale;
        }
        
        public void PlayAppearAnimation(Action callback)
        {
            if (_scaleTween!= null)
            {
                _scaleTween.Kill();
                _scaleTween = null;
            }
            
            _viewTransform.localScale = Vector3.zero;
            _scaleTween = _viewTransform.DOScale(_normalScale, _refreshTime).OnComplete(() => callback?.Invoke());
        }

        public void PlayDisappearAnimation(Action callback)
        {
            if (_scaleTween!= null)
            {
                _scaleTween.Kill();
                _scaleTween = null;
            }
            
            _scaleTween =_viewTransform.DOScale(0, _refreshTime).OnComplete(() => callback?.Invoke());
        }

        public void Cleanup()
        {
            if (_scaleTween!= null)
            {
                _scaleTween.Kill();
                _scaleTween = null;
            }
            
            if (_notificationTween != null)
            {
                _notificationTween.Kill();
                _notificationTween = null;
            }
        }
    }
}
