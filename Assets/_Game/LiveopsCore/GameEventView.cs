using _Game.LiveopsCore._Enums;
using _Game.UI._Currencies;
using _Game.UI.Pin.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.LiveopsCore
{
    public class GameEventView : MonoBehaviour
    {
        public event UnityAction ButtonClicked
        {
            add => _button.onClick.AddListener(value);
            remove => _button.onClick.RemoveListener(value);
        }

        [SerializeField, Required] private RectTransform _rootTransform;
        [SerializeField, Required] private Image _iconHolder;
        [SerializeField, Required] private Button _button;

        [SerializeField, Required] private CurrencyView _currencyView;
        
        [SerializeField, Required] private EventInfoView _infoView;

        [SerializeField, Required] private PinView _pinView;
        [SerializeField] private Vector3 _leftPintViewPosition, _rightPintViewPosition;

        public CurrencyView CurrencyView => _currencyView;

        public void SetIcon(Sprite icon) =>
            _iconHolder.sprite = icon;

        public void SetSiblingIndex(int index) => _rootTransform.SetSiblingIndex(index);
        public int GetSiblingIndex() => _rootTransform.GetSiblingIndex();
        public void SetInteractable(bool isInteractable) => _button.interactable = isInteractable;
        public void SetInfo(string info) => _infoView.SetInfo(info);
        public void SetCurrencyViewActive(bool isActive) => _currencyView.SetActive(isActive);
        public void SetInfoViewActive(bool isActive) => _infoView.SetActive(isActive);
        public void SetPinViewActive(bool isActive) => _pinView.SetActive(isActive);
        public void SetupPinViewPosition(GameEventPanelType type)
        {
            switch (type)
            {
                case GameEventPanelType.Left:
                    _pinView.SetPosition(_rightPintViewPosition);
                    break;
                case GameEventPanelType.Right:
                    _pinView.SetPosition(_leftPintViewPosition);
                    break;
            }
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}