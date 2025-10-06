using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Game.UI.BattlePass.Scripts
{
    public class PopupAppearanceAnimationToolkit
    {
        private readonly VisualElement _target;
        private Tween _scaleTween;
        private Tween _fadeTween;
        
        private const float DURATION = 0.25f;
        private const Ease EASE_TYPE = Ease.OutBack;
        
        private const float EPS_SCALE = 0.001f;
        private const float EPS_ALPHA = 0.001f;
        
        public PopupAppearanceAnimationToolkit(VisualElement target)
        {
            _target = target;
        
            _target.usageHints |= UsageHints.DynamicTransform;
            
            _target.style.visibility = Visibility.Hidden;
            _target.style.opacity = 1;
            _target.transform.scale = Vector3.one * EPS_SCALE;
            
            _target.RegisterCallback<AttachToPanelEvent>(_ =>
            {
                _target.style.visibility = Visibility.Hidden;
                _target.style.opacity = 1;
                _target.transform.scale = Vector3.one * EPS_SCALE;
                _target.MarkDirtyRepaint();
            });
        }
        
        public void Show(Action onComplete = null)
        {
            KillTweens();
        
            _target.style.visibility = Visibility.Visible;
            _target.style.opacity = 1;
            _target.transform.scale = Vector3.one * EPS_SCALE;
        
            _fadeTween = DOVirtual.Float(EPS_ALPHA, 1f, DURATION, v =>
            {
            
                _target.style.opacity = v;
                _target.MarkDirtyRepaint();
            });
        
            _scaleTween = DOTween.To(
                    () => _target.transform.scale,
                    val =>
                    {
                        _target.transform.scale = val;
                        _target.MarkDirtyRepaint();
                    },
                    Vector3.one,
                    DURATION)
                .SetEase(EASE_TYPE)
                .OnComplete(() =>
                {
                    _target.style.visibility = Visibility.Visible;
                    _target.style.opacity = 1;
                    onComplete?.Invoke();
                });
        }
        
        public void Hide(Action onComplete = null)
        {
            KillTweens();
        
            _fadeTween = DOVirtual.Float(1f, EPS_ALPHA, DURATION, v =>
            {
                _target.style.opacity = v;
                _target.MarkDirtyRepaint();
            });
        
            _scaleTween = DOTween.To(
                    () => _target.transform.scale,
                    val =>
                    {
                        _target.transform.scale = val;
                        _target.MarkDirtyRepaint();
                    },
                    Vector3.one * EPS_SCALE,
                    DURATION)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    _target.style.visibility = Visibility.Hidden;
                    _target.style.opacity = 1;
                    onComplete?.Invoke();
                });
        }
        
        private void KillTweens()
        {
            _fadeTween?.Kill();
            _scaleTween?.Kill();
            _fadeTween = null;
            _scaleTween = null;
        }
        
        public void Dispose() => KillTweens();


        // private readonly VisualElement _target;
        // private Tween _scaleTween;
        // private Tween _fadeTween;
        //
        // private const float DURATION = 0.25f;
        // private const Ease SCALE_EASE = Ease.OutBack;
        // private const Ease FADE_EASE = Ease.OutQuad;
        //
        // private const float EPS_SCALE = 0.001f;
        // private const float EPS_ALPHA = 0.001f;
        //
        // private bool _isShownPending;
        //
        // public PopupAppearanceAnimationToolkit(VisualElement target)
        // {
        //     _target = target;
        //
        //     _target.usageHints |= UsageHints.DynamicTransform;
        //     
        //     _target.style.display = DisplayStyle.None;
        //     _target.style.opacity = EPS_ALPHA;
        //     _target.transform.scale = Vector3.one * EPS_SCALE;
        //     
        //     _target.RegisterCallback<AttachToPanelEvent>(_ =>
        //     {
        //         if (_isShownPending)
        //             StartShowTweens();
        //     });
        // }
        //
        // public void Show(Action onComplete = null)
        // {
        //     KillTweens();
        //
        //     _isShownPending = true;
        //     
        //     _target.style.display = DisplayStyle.Flex;
        //     _target.style.opacity = EPS_ALPHA;
        //     _target.transform.scale = Vector3.one * EPS_SCALE;
        //     
        //     void handler(GeometryChangedEvent _)
        //     {
        //         _target.UnregisterCallback<GeometryChangedEvent>(handler);
        //         StartShowTweens(onComplete);
        //     }
        //     
        //     
        //     if (_target.panel != null)
        //         _target.RegisterCallback<GeometryChangedEvent>(handler);
        //     else
        //         _target.RegisterCallback<AttachToPanelEvent>(_ =>
        //             _target.RegisterCallback<GeometryChangedEvent>(handler));
        // }
        //
        // private void StartShowTweens(Action onComplete = null)
        // {
        //     _isShownPending = false;
        //
        //     _fadeTween = DOVirtual.Float(EPS_ALPHA, 1f, DURATION, v => { _target.style.opacity = v; })
        //         .SetEase(FADE_EASE);
        //
        //     _scaleTween = DOTween.To(
        //             () => _target.transform.scale,
        //             val => { _target.transform.scale = val; },
        //             Vector3.one,
        //             DURATION)
        //         .SetEase(SCALE_EASE)
        //         .OnComplete(() =>
        //         {
        //             _target.style.opacity = 1f;
        //             onComplete?.Invoke();
        //         });
        // }
        //
        // public void Hide(Action onComplete = null)
        // {
        //     KillTweens();
        //
        //     _fadeTween = DOVirtual
        //         .Float(_target.resolvedStyle.opacity, EPS_ALPHA, DURATION, v => { _target.style.opacity = v; })
        //         .SetEase(FADE_EASE);
        //
        //     _scaleTween = DOTween.To(
        //             () => _target.transform.scale,
        //             val => { _target.transform.scale = val; },
        //             Vector3.one * EPS_SCALE,
        //             DURATION)
        //         .SetEase(Ease.InBack)
        //         .OnComplete(() =>
        //         {
        //             _target.style.display = DisplayStyle.None;
        //             _target.style.opacity = EPS_ALPHA;
        //             onComplete?.Invoke();
        //         });
        // }
        //
        // private void KillTweens()
        // {
        //     _fadeTween?.Kill();
        //     _scaleTween?.Kill();
        //     _fadeTween = null;
        //     _scaleTween = null;
        // }
        //
        // public void Dispose() => KillTweens();
    }
}