using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI.Common.Scripts
{
    [RequireComponent(typeof(Button))]
    public class ToggleWithSpriteSwap : MonoBehaviour
    {
        public event UnityAction ButtonClicked
        {
            add => _button.onClick.AddListener(value);
            remove => _button.onClick.RemoveListener(value);
        }

        [SerializeField] private Sprite _offSprite;
        [SerializeField] private Sprite _onSprite;
        [SerializeField] private Image _changableImage;
        [SerializeField] private Button _button;

        public void SetActive(bool isAvailable) => 
            gameObject.SetActive(isAvailable);

        public void SetPaused(bool isOn) => _changableImage.sprite = isOn ? _onSprite : _offSprite;

        public void SetInteractable(bool isInteractable) => _button.interactable = isInteractable;
    }
}