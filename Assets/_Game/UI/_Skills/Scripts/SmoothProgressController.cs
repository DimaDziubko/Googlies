using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Skills.Scripts
{
    public class SmoothProgressController : MonoBehaviour
    {
        [SerializeField, Required] private Image _progressBar;
        
        private float _targetProgress;
        private float _startProgress;
        private float _duration;
        private float _elapsedTime;
        private bool _isAnimating = false;

        public void SetActive(bool isActive) => _progressBar.gameObject.SetActive(isActive);

        public void SetProgressSmooth(float startProgress, float targetProgress, float duration)
        {
            _startProgress = startProgress;
            _targetProgress = targetProgress;
            _duration = duration;
            _elapsedTime = 0f;
            _isAnimating = true;
        }

        public void Tick(float deltaTime)
        {
            if (!_isAnimating)
                return;

            _elapsedTime += deltaTime;
            float t = Mathf.Clamp01(_elapsedTime / _duration);
            
            _progressBar.fillAmount = Mathf.Lerp(_startProgress, _targetProgress, t);
            
            if (t >= 1f)
                _isAnimating = false;
        }
    }
}