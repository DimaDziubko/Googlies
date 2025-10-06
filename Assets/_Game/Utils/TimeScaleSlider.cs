using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Utils
{
    internal class TimeScaleSlider : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _valueText;


        private void OnEnable()
        {
            _slider.onValueChanged.AddListener(OnSliderValueChanged);
        }

        private void Start()
        {
            if (_slider == null || _valueText == null)
            {
                Debug.LogError("Slider или TextMeshPro не назначены!");
                return;
            }

            _slider.wholeNumbers = true;

            _slider.value = Time.timeScale;
            UpdateText(_slider.value);
        }

        private void OnSliderValueChanged(float value)
        {
            Time.timeScale = value;
            UpdateText(value);
        }

        private void UpdateText(float value)
        {
            _valueText.text = $"Speed: {value:0}";
        }

        private void OnDisable()
        {
            _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
}
