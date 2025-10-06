using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Shop.Scripts._AmountView
{
    public sealed class AmountView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _amount;

        public void SetAmount(string amount) => _amount.text = amount;
        public void SetIcon(Sprite icon) => _icon.sprite = icon;
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        public void SetActive(bool isActive) => 
            gameObject.SetActive(isActive);
    }
}