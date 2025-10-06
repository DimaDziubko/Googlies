using _Game.Gameplay._Units.Scripts;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.Gameplay._UnitBuilder.Scripts
{
    public class UnitBuildButton : MonoBehaviour
    {
        public event UnityAction Clicked
        {
            add => _button.onClick.AddListener(value);
            remove => _button.onClick.RemoveListener(value);
        }

        [SerializeField] private ThemedButton _button;
        [SerializeField] private Image _unitIconHolder;
        [SerializeField] private Image _foodIconHolder;
        [SerializeField] private TMP_Text _priceLabel;
        [SerializeField] private GameObject _btnContainer;
        [SerializeField] private UnitBuilderBtnScaleAnimation _animation;
        
        [SerializeField] private UnitType _type;
        public UnitType Type => _type;
        public void SetUnitIcon(Sprite icon) => 
            _unitIconHolder.sprite = icon;
        public void SetUnitIconColor(Color color) => 
            _unitIconHolder.color = color;
        public void SetFoodIcon(Sprite icon) => 
            _foodIconHolder.sprite = icon;
        public void SetPrice(string price) => 
            _priceLabel.text = price;
        public void SetInteractable(bool isInteractable)
        {
            if (!_button.interactable && isInteractable)
            {
                _animation.DoScaleAnimation();
            }
            
            _button.SetInteractable(isInteractable);
        }

        public void SetActive(bool isActive) => 
            _btnContainer.SetActive(isActive);
    }
}