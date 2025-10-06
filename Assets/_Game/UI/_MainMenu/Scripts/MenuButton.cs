using _Game.UI.Pin.Scripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI._MainMenu.Scripts
{
    public class MenuButton : MonoBehaviour
    {
        public event UnityAction ButtonClicked
        {
            add => _button.onClick.AddListener(value);
            remove => _button.onClick.RemoveListener(value);
        }

        [SerializeField] private RectTransform _rectTransform;

        [SerializeField] private Button _button;
        [SerializeField] private PinView _pin;
        [SerializeField] private Image _highlightedImage;
        
        [SerializeField] private Image _iconHolder;
        [SerializeField] private Sprite _activeIcon;
        [SerializeField] private Sprite _lockedIcon;
        
        [SerializeField] private MenuButtonType _type;
        
        public PinView Pin => _pin;
        public MenuButtonType Type => _type;

        public RectTransform RectTransform => _rectTransform;

        public void SetupPin(bool isActive) => _pin.SetActive(isActive);

        public void SetLocked(bool isLocked)
        {
            _iconHolder.sprite = isLocked ? _lockedIcon : _activeIcon;
        }

        public void SetInteractable(bool isInteractable) => 
            _button.interactable = isInteractable;
         
        public void SetHighlighted(bool isHighlighted) => 
            _highlightedImage.enabled = isHighlighted;

        public void SetSize(Vector2 size) => 
            _rectTransform.sizeDelta = size;
    }
}