using Sirenix.OdinInspector;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI._Skills.Scripts
{
    public class SkillSocket : MonoBehaviour
    {
        public event UnityAction OnSkillClicked
        {
            add => _button.onClick.AddListener(value);
            remove => _button.onClick.RemoveListener(value);
        }
        
        [SerializeField] private int _id;
        [SerializeField, Required] private ThemedButton _button;
        
        [SerializeField, Required] private GameObject _equippedView;
        [SerializeField, Required] private Image _iconHolder;
        [SerializeField, Required] private SmoothProgressController _smoothProgressController;
        
        [SerializeField] private Color _interactableIconColor;
        [SerializeField] private Color _notInteractableIconColor;
        
        [SerializeField, Required] private GameObject _unequippedView;
        
        public int Id => _id;
        public SmoothProgressController SmoothProgressController => _smoothProgressController;
        
        public void SetActive(bool active) => 
            gameObject.SetActive(active);

        public void SetEquipped(bool isEquipped)
        {
            _equippedView.SetActive(isEquipped);
            _unequippedView.SetActive(!isEquipped);
        }

        public void SetIcon(Sprite icon) => 
            _iconHolder.sprite = icon;

        public void SetInteractable(bool isInteractable)
        {
            _button.SetInteractable(isInteractable);
            _iconHolder.color = isInteractable ? _interactableIconColor : _notInteractableIconColor;
        }
    }
}