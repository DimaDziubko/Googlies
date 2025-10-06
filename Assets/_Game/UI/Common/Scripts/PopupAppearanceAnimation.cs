using System;
using UnityEngine;

namespace _Game.UI.Common.Scripts
{
    [RequireComponent(typeof(FadeComponent), typeof(ScaleAnimationComponent))]
    public class PopupAppearanceAnimation : MonoBehaviour
    {
        [SerializeField] private FadeComponent _fadeComponent;
        [SerializeField] private ScaleAnimationComponent _scaleAnimationComponent;

        private bool _isAnimating;

        public void PlayShow(Action onComplete = null)
        {
            Cleanup();

            _isAnimating = true;
            int completedAnimations = 0;

            Action onSingleAnimationComplete = () =>
            {
                completedAnimations++;
                if (completedAnimations == 2)
                {
                    _isAnimating = false;
                    onComplete?.Invoke();
                }
            };

            _fadeComponent?.FadeIn(onSingleAnimationComplete);
            _scaleAnimationComponent?.ScaleIn(onSingleAnimationComplete);
        }

        public void PlayHide(Action onComplete = null)
        {
            Cleanup();

            _isAnimating = true;
            int completedAnimations = 0;

            Action onSingleAnimationComplete = () =>
            {
                completedAnimations++;
                if (completedAnimations == 2)
                {
                    _isAnimating = false;
                    onComplete?.Invoke();
                }
            };

            _scaleAnimationComponent?.ScaleOut(onSingleAnimationComplete);
            _fadeComponent?.FadeOut(onSingleAnimationComplete);
        }

        public void Cleanup()
        {
            if (_isAnimating)
            {
                _fadeComponent?.KillAnimations();
                _scaleAnimationComponent?.KillAnimations();

                _isAnimating = false;
            }
        }
    }
}