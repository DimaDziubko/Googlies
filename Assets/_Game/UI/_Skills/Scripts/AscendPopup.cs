using _Game.UI.Common.Scripts;
using Sirenix.OdinInspector;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI._Skills.Scripts
{
    public class AscendPopup : MonoBehaviour
    {
        public event UnityAction OnAscendButtonClicked
        {
            add => _ascendButton.onClick.AddListener(value);
            remove => _ascendButton.onClick.RemoveListener(value);
        }
        
        public event UnityAction OnCancelButtonClicked
        {
            add
            {
                foreach (var button in _cancelButtons)
                {
                    button.onClick.AddListener(value);
                }
            }
            remove
            {
                foreach (var button in _cancelButtons)
                {
                    button.onClick.RemoveListener(value);
                }
            }
        }

        [SerializeField, Required] private Button[] _cancelButtons;
        [SerializeField, Required] private ThemedButton _ascendButton;
        [SerializeField, Required] private PassiveBoostInfoListView _passiveBoostInfoListView;
        [SerializeField, Required] private PopupAppearanceAnimation _popupAppearanceAnimation;
        
        public PassiveBoostInfoListView PassiveBoostInfoListView => _passiveBoostInfoListView;
        public PopupAppearanceAnimation PopupAppearanceAnimation => _popupAppearanceAnimation;
        
        public void SetAscendButtonInteractable(bool isInteractable) => 
            _ascendButton.SetInteractable(isInteractable);

        public void SetActive(bool isActive) => 
            gameObject.SetActive(isActive);
    }
}