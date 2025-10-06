using System;
using _Game.Core.Services.IAP;
using _Game.LiveopsCore.Models.ClassicOffer;
using _Game.UI.ClassicOffers.DailyChallenge.Scripts;
using _Game.Utils.Extensions;
using UnityEngine.Purchasing;
using UnityUtils;

namespace _Game.UI.ClassicOffers.Scripts.Presenter
{
    public class ClassicOfferPopupPresenter : IDisposable
    {
        //public event Action<float> EventTimerTick;

        private readonly IAPProvider _iapProvider;

        private ClassicOfferEvent _event;
        private ClassicOfferPopup _popup;

        public float CurrentEventTimerTimeSeconds => _event.EventTimeLeft;


        public ClassicOfferPopupPresenter(
            ClassicOfferEvent @event,
            ClassicOfferPopup popup,
            IAPProvider iAPProvider
            )
        {
            _event = @event;
            _popup = popup;

            _iapProvider = iAPProvider;

            UnSubsribe();
        }

        public void SetModel(ClassicOfferEvent @event) =>
            _event = @event;
        public void SetPopup(ClassicOfferPopup popup) =>
            _popup = popup;

        public void Initialize()
        {
            Subsribe();
        }
        public void Dispose()
        {
            UnSubsribe();
            ClosePopup();
        }

        public Product GetSegmentedIAP() // Mb Meditator/stategy or something for all events
        {
            var segmentedProduct = _event.SegmentIAP();
            if (segmentedProduct == null)
            {
                return null;
            }

            _iapProvider.AllProducts.TryGetValue(segmentedProduct.ProductId, out var product);

            return product;
        }

        public void InitValues()
        {
            if (_popup.OrNull() != null)
            {
                //CheckProduct();
                InitItems();
                _popup.SetAvalivableText($"Avaliable {_event.GetAvalivableCount}/{_event.PurchaseLimit}");
                _popup.SetDiscountText($"<size=45>{_event.Discount}%</size>\r\nBonus");
                _popup.SetHeaderText($"{_event.Name}");
                OnEventTimerTick(_event.EventTimeLeft);

                var product = GetSegmentedIAP();

                if (product != null)
                {
                    _popup.SetPurchaseButton(product.metadata.localizedPriceString);
                }
            }
        }

        private void InitItems()
        {
            foreach (var reward in _event.Rewards)
            {
                var itemView = _popup.SpawnItemView();

                itemView.StateInfoButton(false);

                itemView.SetAmount(reward.Amount.ToString());
                itemView.SetIcon(reward.Icon);
            }
        }

        public void ClosePopup()
        {
            if (_popup != null)
                _popup.Close();
        }

        private void OnEventTimerTick(float secondsLeft)
        {
            _popup.SetTimerText(TimeSpan.FromSeconds(secondsLeft).ToCondensedTimeFormat());
            // EventTimerTick?.Invoke(secondsLeft);
        }

        private void UnSubsribe()
        {
            _event.OnEventTimerTick -= OnEventTimerTick;
            //_event.OnOfferPurchased -= ClosePopup;
            _popup.OnPurchase -= TryToPurchaseOffer;
            _event.OnOfferPurchased -= ProgressChanged;
        }

        private void Subsribe()
        {
            _event.OnEventTimerTick += OnEventTimerTick;
            //_event.OnOfferPurchased += ClosePopup;
            _popup.OnPurchase += TryToPurchaseOffer;
            _event.OnOfferPurchased += ProgressChanged;
        }

        private void ProgressChanged()
        {
            if (_popup != null)
            {
                _popup.SetAvalivableText($"Avaliable {_event.GetAvalivableCount}/{_event.PurchaseLimit}");
                if (_event.GetAvalivableCount <= 0)
                    ClosePopup();
            }
        }

        private void TryToPurchaseOffer()
        {
            //if (_event.Bundle != null)
            //{
            _iapProvider.StartPurchase(GetSegmentedIAP().definition.id);
            //}
        }

        //private void CheckProduct()
        //{
        //    if (_event.Bundle != null)
        //    {
        //        if (_event.Bundle.Product == null)
        //            _event.Bundle.TryUpdateProduct();
        //    }
        //}
    }
}
