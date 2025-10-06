using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Gameplay.Food.Scripts
{
    public class FoodPanel : MonoBehaviour
    {
        [SerializeField] private Slider _foodSlider;
        [SerializeField] private Image _foodIconHolder;
        [SerializeField] private TMP_Text _foodAmountLabel;

        private const float EPSILON = 0.01f;
        
        private bool _isAnimating;
        private float _targetValue;
        
        [ShowInInspector]
        private float _animationSpeed;

        public void Tick(float deltaTime)
        {
            if (_isAnimating)
            {
                _foodSlider.value = Mathf.MoveTowards(_foodSlider.value, _targetValue, _animationSpeed * deltaTime);
                
                if (Mathf.Abs(_foodSlider.value - 1f) < EPSILON)
                {
                    _foodSlider.value = 1f;
                    _isAnimating = false;
                }
      
                else if (Mathf.Approximately(_foodSlider.value, _targetValue))
                {
                    _isAnimating = false;
                }
            }
        }
        
        public void SetupIcon(Sprite foodIcon) => 
            _foodIconHolder.sprite = foodIcon;

        public void UpdateFillAmount(float progress, float productionSpeed)
        {
            if (progress < _foodSlider.value)
            {
                _foodSlider.value = progress;
                _isAnimating = false;
            }
            else
            {
                _targetValue = progress;
                _animationSpeed = productionSpeed;
                _isAnimating = true;
            }
        }

        public void OnFoodChanged(int amount) => 
            _foodAmountLabel.text = amount.ToString();
    }
}