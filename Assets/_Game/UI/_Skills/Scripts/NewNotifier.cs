using System;
using _Game.UI.Common.Scripts;
using UnityEngine;

namespace _Game.UI._Skills.Scripts
{
    public class NewNotifier : MonoBehaviour
    {
        [SerializeField] private ScaleAnimationComponent _animation;

        private bool _isAnimating;
        
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
        
        public void PlayPulseAnimation(Action callback = null)
        {
            Cleanup();
            _animation.ResetSelf();
            _isAnimating = true;
            _animation.PulseAnimation(callback);
        }
        
        public void StopPulseAnimation(Action callback = null)
        {
            _animation.StopPulse();
        }

        public void Cleanup()
        {
            if (_isAnimating)
            {
                _animation?.KillAnimations(); 
            }
        }
    }
}