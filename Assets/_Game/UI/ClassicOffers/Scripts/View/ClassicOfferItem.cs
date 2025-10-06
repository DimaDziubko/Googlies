using _Game.UI._Shop.Scripts._AmountView;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.ClassicOffers.DailyChallenge.Scripts
{
    public class ClassicOfferItem : MonoBehaviour
    {
        [SerializeField] private AmountView _amountView;
        [SerializeField, Required] private Button _infoButton;

        public void StateInfoButton(bool isActive) => _infoButton.gameObject.SetActive(isActive);
        public void SetAmount(string amount) => _amountView.SetAmount(amount);
        public void SetIcon(Sprite icon) => _amountView.SetIcon(icon);
    }
}
