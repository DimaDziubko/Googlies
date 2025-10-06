using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.Common.Scripts
{
    [RequireComponent(typeof(Slider))]
    public class SliderHelper : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        private void OnValidate()
        {
            _slider = GetComponent<Slider>();
        }
        //Not need, don't fix our problem (
        //private void OnEnable()
        //{
        //    FixSliderFillRect();
        //}

        //private void Awake()
        //{
        //    FixSliderFillRect();
        //}
        //private void Start()
        //{
        //    FixSliderFillRect();
        //}

        public void FixSliderFillRect()
        {
            //Debug.Log("SliderHelper " + _slider.fillRect.anchoredPosition);
            if (_slider.fillRect.anchoredPosition != Vector2.zero)
            {
                _slider.fillRect.anchoredPosition = Vector2.zero;
                Debug.Log("SliderHelper FIX! " + _slider.fillRect.anchoredPosition);
            };
        }
    }
}