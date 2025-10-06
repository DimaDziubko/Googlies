using System;
using _Game.UI.Common.Scripts;
using UnityEngine;

namespace _Game.UI._Skills.Scripts
{
    public class SplashImage : MonoBehaviour
    {
        [SerializeField] private ScaleAnimationComponent _splashImageScaleComponent;
        [SerializeField] private ImageFadeComponent _splashImageFadeComponent;

        private bool _isAnimating;
        
        public void PlayAnimation(Action callback)
        {
            Cleanup();
            _splashImageFadeComponent.ResetSelf();
            _splashImageScaleComponent.ResetSelf();
            
            _isAnimating = true;
            int completedAnimations = 0;

            Action onSingleAnimationComplete = () =>
            {
                completedAnimations++;
                if (completedAnimations == 2)
                {
                    _isAnimating = false;
                    callback?.Invoke();
                }
            };

            _splashImageScaleComponent?.ScaleIn(onSingleAnimationComplete);
            _splashImageFadeComponent?.FadeIn(onSingleAnimationComplete);
        }

        public void Cleanup()
        {
            if (_isAnimating)
            {
                _splashImageFadeComponent?.KillAnimations();
                _splashImageScaleComponent?.KillAnimations();
                
                _isAnimating = false;
            }
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}