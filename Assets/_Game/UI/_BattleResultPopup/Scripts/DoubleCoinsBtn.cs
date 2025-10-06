using Sirenix.OdinInspector;
using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI._BattleResultPopup.Scripts
{
    public class DoubleCoinsBtn : MonoBehaviour
    {
        public event UnityAction ButtonClicked
        {
            add => _button.onClick.AddListener(value);
            remove => _button.onClick.RemoveListener(value);
        }
        
        [SerializeField, Required] private TMP_Text _loadingText;
        [SerializeField, Required] private TMP_Text _x2Text;
        [SerializeField, Required] private Image _adsIconHolder;
        [SerializeField, Required] private ThemedButton _button;
        
        public void SetInteractable(bool isInteractable)
        {
            _button.SetInteractable(isInteractable);
            _adsIconHolder.enabled = isInteractable;
            _x2Text.enabled = isInteractable;

            _loadingText.enabled = !isInteractable;
        }
        
    }
}