using _Game.UI.ClassicOffers.Scripts;
using TMPro;
using UnityEngine;

namespace _Game.UI.ClassicOffers.DailyChallenge.Scripts
{
    public sealed class ClassicOfferPopup : ClassicOfferViewBase
    {
        [Space]
        [SerializeField] private TextMeshProUGUI _headerLaybel;
        [SerializeField] private ClassicOfferListView _listView;
        [Space]
        [SerializeField] private TextMeshProUGUI _discountLaybel;
        [SerializeField] private TextMeshProUGUI _avalivableLaybel;
        [SerializeField] private TextMeshProUGUI _timerLaybel;

        public ClassicOfferItem SpawnItemView()
        {
            return _listView.SpawnElement();
        }

        public void RemoveItemsView()
        {
            _listView.Clear();
        }

        public void RemoveItemView(ClassicOfferItem item)
        {
            _listView.DestroyElement(item);
        }

        public void SetHeaderText(string text) => _headerLaybel.text = text;
        public void SetDiscountText(string text) => _discountLaybel.text = text;
        public void SetAvalivableText(string text) => _avalivableLaybel.text = text;
        public void SetTimerText(string text) => _timerLaybel.text = text;

    }
}
