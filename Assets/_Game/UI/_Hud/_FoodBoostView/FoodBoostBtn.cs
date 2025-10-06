using TMPro;
using Unity.Theme.Binders;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace _Game.UI._Hud._FoodBoostView
{
    public class FoodBoostBtn : MonoBehaviour
    {
        public event UnityAction ButtonClicked
        {
            add => _button.onClick.AddListener(value);
            remove => _button.onClick.RemoveListener(value);
        }

        [SerializeField] private GameObject _activePanel;
        [SerializeField] private TMP_Text _loadingText;
        
        [SerializeField] private Image _adsIconHolder;
        [SerializeField] private TMP_Text _foodAmountLabel;
        [SerializeField] private Image _foodIconHolder;

        [SerializeField] private ThemedButton _button;
        
        public void SetValue(string value) => _foodAmountLabel.text = value;
        public void SetIcon(Sprite icon) => _foodIconHolder.sprite = icon;

        public void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
            _loadingText.enabled = !isInteractable;
            _activePanel.SetActive(isInteractable);
        }
        
        public void SetActive(bool isActive) => gameObject.SetActive(isActive);
    }
}