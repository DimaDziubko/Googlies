using System;
using _Game.UI.Common.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Skills.Scripts
{
    public class SkillView : MonoBehaviour
    {
        [SerializeField] private Image _iconPlaceholder;
        
        [SerializeField] private Image _animationBg;
        
        [SerializeField] private NewNotifier _newNotifier;
        [SerializeField] private SplashImage _splashImage;
        
        [SerializeField] private ImageFadeComponent _iconFadeAnimation;
        [SerializeField] private ImageFadeComponent _bgFadeAnimation;
        
        private bool _isAnimating;
        
        public void SetIcon(Sprite icon)
        {
            _iconPlaceholder.sprite = icon;
        }
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
        
        public void SetSplashImageActive(bool isActive)
        {
            _splashImage.SetActive(isActive);
        }

        public void SetAnimationBgActive(bool isActive)
        {
            _animationBg.enabled = isActive;
        }

        public void SetNewNotifierActive(bool isActive)
        {
            _newNotifier.SetActive(isActive);
        }

        [Button]
        public void PlaySimpleAnimation(Action callback = null)
        {
            Cleanup();
            _isAnimating = true;
            _iconFadeAnimation.ResetSelf();
            _bgFadeAnimation.ResetSelf();
            
            _newNotifier.SetActive(false);
            _splashImage.SetActive(false);
            
            _bgFadeAnimation?.FadeIn(() => PlayIconFadeAnimation(callback));
        }

        private void PlayIconFadeAnimation(Action callback)
        {
            _iconFadeAnimation?.FadeIn(() =>
            {
                _isAnimating = false;
                callback?.Invoke();
            });
        }

        [Button]
        public void PlayIlluminationAnimation(Action callback = null)
        {
            Cleanup();
            _isAnimating = true;
            _iconFadeAnimation.ResetSelf();
            _bgFadeAnimation.ResetSelf();
            _newNotifier.SetActive(false);
            _splashImage.SetActive(false);
            _bgFadeAnimation?.FadeIn(() => PlayIlluminationAndNewNotification(callback));
        }

        private void PlayIlluminationAndNewNotification(Action callback = null)
        {
            callback?.Invoke();
            
            int animationsToComplete = 3;
            int completedAnimations = 0;

            Action onSingleAnimationComplete = () =>
            {
                completedAnimations++;
                if (completedAnimations >= animationsToComplete)
                {
                    _isAnimating = false;
                }
            };

            _newNotifier?.SetActive(true);
            _splashImage.SetActive(true);
            _iconFadeAnimation.FadeIn(onSingleAnimationComplete);
            _newNotifier?.PlayPulseAnimation(onSingleAnimationComplete);
            _splashImage?.PlayAnimation(onSingleAnimationComplete);
        }

        public void Cleanup()
        {
            if (_isAnimating)
            {
                _bgFadeAnimation?.KillAnimations();
                _iconFadeAnimation?.KillAnimations();
                _splashImage?.Cleanup();
                _newNotifier?.Cleanup();
                
                _isAnimating = false;
            }
        }
    }
}