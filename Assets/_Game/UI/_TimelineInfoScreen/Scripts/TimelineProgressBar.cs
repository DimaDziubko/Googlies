using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._TimelineInfoScreen.Scripts
{
    public class TimelineProgressBar : MonoBehaviour
    {
        private const float _sliderCoefficient = 0.825f;

        [SerializeField] private RectTransform _transform;
        [SerializeField] private Slider _slider;

        private Tween _sliderTween;

        public void UpdateValue(int currentAge, int ages)
        {
            Debug.Log($"[UpdateValue] currentAge: {currentAge}, ages: {ages}");

            float percentage = 0;
            if (ages > 0)
            {
                percentage = (float)currentAge / (ages - 1) * _sliderCoefficient;
                Debug.Log($"[UpdateValue] percentage calculated: {percentage}");
            }
            else
            {
                Debug.LogWarning("[UpdateValue] ages <= 0, percentage = 0");
            }

            float clampedValue = Mathf.Clamp01(percentage);
            Debug.Log($"[UpdateValue] clamped value: {clampedValue}, slider value before: {_slider.value}");

            _slider.value = clampedValue;

            Debug.Log($"[UpdateValue] slider value after: {_slider.value}");
        }

        public void AdjustScrollPositionToAge(int currentAge)
        {
            _slider.value = currentAge;
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

        public void SetAnchoredPosition(float firstPresenterX) => _transform.anchoredPosition = new Vector2(firstPresenterX, _transform.anchoredPosition.y);

        public void Cleanup()
        {
            _sliderTween?.Kill();
            _sliderTween = null;
        }
    }
}