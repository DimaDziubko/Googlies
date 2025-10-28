using _Game.UI.Pin.Scripts;
using Sirenix.OdinInspector;
using Unity.Theme;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI.Common.Scripts
{
    [RequireComponent(typeof(Button))]
    public class SimpleToggleButton : MonoBehaviour
    {
        public event UnityAction ButtonClicked
        {
            add => _button.onClick.AddListener(value);
            remove => _button.onClick.RemoveListener(value);
        }

        [SerializeField, Required] private Button _button;
        [SerializeField, Required] private PinView _pinView;
        [SerializeField, Required] private Image _changableImage;
        //[SerializeField, Required] private ImageColorBinder _imageColorBinder;
        [SerializeField, Required] private Sprite _inactiveSprite;
        [SerializeField, Required] private Sprite _activeSprite;

        //[Title("First - Highlited/ Second - UNHighlighted")]
        //[SerializeField] private ColorBinderData _highlightedColorBinder;
        //[SerializeField] private ColorBinderData _unHighlightedColorBinder;

        public PinView Pin => _pinView;

        public void SetupPin(bool isActive) =>
            _pinView.SetActive(isActive);

        public void SetInteractable(bool isInteractable) =>
            _button.interactable = isInteractable;

        public void Highlight()
        {
            _changableImage.sprite = _activeSprite;

            //if (_highlightedColorBinder != null)
            //{
            //    _imageColorBinder.SetColorByName(_highlightedColorBinder.ColorName);
            //}

            SetInteractable(false);
        }

        public void UnHighlight()
        {
            _changableImage.sprite = _inactiveSprite;

            //if (_unHighlightedColorBinder != null)
            //{
            //    _imageColorBinder.SetColorByName(_unHighlightedColorBinder.ColorName);
            //}

            SetInteractable(true);
        }
    }
}