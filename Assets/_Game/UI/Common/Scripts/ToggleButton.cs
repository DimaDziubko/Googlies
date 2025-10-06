using _Game.UI.Pin.Scripts;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI.Common.Scripts
{
    [RequireComponent(typeof(Button), typeof(ButtonScaleAnimator))]
    public class ToggleButton : MonoBehaviour
    {
        public event UnityAction ButtonClicked
        {
            add => _button.onClick.AddListener(value);
            remove => _button.onClick.RemoveListener(value);
        }
        
        [SerializeField, Required] private Button _button;
        [SerializeField, Required] private GameObject _lockedPanel;
        [SerializeField, Required] private TMP_Text _unlockedText;
        [SerializeField, Required] private TMP_Text _lockedText;
        [SerializeField, Required] private Image _highlightImage;
        [SerializeField, Required] private ButtonScaleAnimator _scaleAnimator;
        [SerializeField, Required] private PinView _pinView;

        [SerializeField, Required] private Image _changableImage;
        [SerializeField, Required] private Sprite _inactiveSprite;
        [SerializeField, Required] private Sprite _activeSprite;
        public PinView Pin => _pinView;

        public void SetupPin(bool isActive) => 
            _pinView.SetActive(isActive);

        public void SetInteractable(bool isInteractable) => 
            _button.interactable = isInteractable;

        public void SetLockedText(string text) =>
            _lockedText.text = text;

        public void SetLocked(bool isLocked)
        {
            _lockedPanel.SetActive(isLocked);
            _unlockedText.enabled = !isLocked;
            _changableImage.sprite = _inactiveSprite;
        }

        public void Highlight()
        {
            _highlightImage.enabled = true;
            _scaleAnimator.Highlight();
            _changableImage.sprite = _activeSprite;
        }

        public void UnHighlight()
        {
            _highlightImage.enabled = false;
            _scaleAnimator.UnHighlight();
            _changableImage.sprite = _inactiveSprite;
        }
    }
}