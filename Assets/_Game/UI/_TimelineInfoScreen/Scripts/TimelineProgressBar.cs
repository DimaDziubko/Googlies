using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._TimelineInfoScreen.Scripts
{
    public class TimelineProgressBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _transform;
        [SerializeField] private Slider _slider;

        private Tween _sliderTween;

        public void UpdateValue(int currentAge, int ages)
        {
            float percentage = 0;
            if (ages > 0)
            {
                percentage = (float) currentAge / (ages - 1);
            }

            _slider.value = Mathf.Clamp01(percentage);
        }

        public Tween PlayValueAnimation(float newValue, float duration)
        {
            _sliderTween?.Kill();
            _sliderTween = _slider.DOValue(newValue, duration);
            return _sliderTween;
        }

        public void SetActive(bool isActive) => 
            gameObject.SetActive(isActive);

        public void SetWidth(float width)
        {
            var size = _transform.sizeDelta;
            size.x = width;
            _transform.sizeDelta = size;
        }

        public void Cleanup()
        {
            _sliderTween?.Kill();
            _sliderTween = null;
        }
    }
}