using _Game.Core._Logger;
using _Game.Core.Services.Analytics;
using _Game.Core.Services.IAP;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.LiveopsCore._Trackers;
using _Game.LiveopsCore.Models.ClassicOffer;
using DevToDev.Analytics;
using UnityEngine.Purchasing;

namespace _Game.Core._GameEventInfrastructure._Trackers
{
    public class ClassicOfferTracker : IGameEventTracker
    {
        private readonly ClassicOfferEvent _event;
        private readonly IUserContainer _userContainer;
        private readonly IMyLogger _logger;
        private readonly IAPProvider _iapProvider;
        private readonly IAnalyticsEventSender _sender;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public ClassicOfferTracker(
            ClassicOfferEvent @event,
            IUserContainer userContainer,
            IMyLogger logger,
            IAnalyticsEventSender sender,
            IAPProvider iapProvider
            )
        {
            _sender = sender;
            _event = @event;
            _userContainer = userContainer;
            _logger = logger;
            _iapProvider = iapProvider;
        }

        void IGameEventTracker.Initialize()
        {
            _event.OnOfferPurchased += TrackPurchase;
            _event.OnShowed += Showed;
            Showed();
        }

        void IGameEventTracker.Dispose()
        {
            _event.OnOfferPurchased -= TrackPurchase;
            _event.OnShowed -= Showed;
        }

        private void TrackOpened()
        {
            if (_event.IsLocked) return;

            var parameters = new DTDCustomEventParameters();

            parameters.Add("ID", _event.Id); //“1602”
            parameters.Add("RewardListID", _event.RewardListID);

            parameters.Add("TimelineID", TimelineState.TimelineNumber);
            parameters.Add("AgeID", TimelineState.AgeNumber);
            parameters.Add("BattleID", TimelineState.BattleNumber);
            parameters.Add("Level", TimelineState.Level);

            _sender.CustomEvent("classic_offer_showed", parameters);
            _event.SetStartSend(true);
        }

        public void TrackPurchase()
        {
            _logger.Log("[ClassicOffer] Tracking purchase...");

            var parameters = new DTDCustomEventParameters();

            parameters.Add("ID", _event.Id); // “1602”
            parameters.Add("RewardListID", _event.RewardListID);

            var segmentedProduct = GetSegmentedIAP();
            var price = segmentedProduct?.metadata?.localizedPriceString ?? "Unknown";
            // или
            price = segmentedProduct?.metadata?.localizedPriceString ?? "0";
            // или
            price = segmentedProduct?.metadata?.localizedPriceString ?? "N/A";

            parameters.Add("Price", price);
            parameters.Add("TimelineID", TimelineState.TimelineNumber);
            parameters.Add("AgeID", TimelineState.AgeNumber);
            parameters.Add("BattleID", TimelineState.BattleNumber);
            parameters.Add("Level", TimelineState.Level);

            _logger.Log($"[ClassicOffer] Purchase Params => ID: {_event.Id}, RewardListID: {_event.RewardListID}, Price: {price}, Timeline: {TimelineState.TimelineNumber}, Age: {TimelineState.AgeNumber}, Battle: {TimelineState.BattleNumber}, Level: {TimelineState.Level}");

            _sender.CustomEvent("classic_offer_purchased", parameters);
        }


        private void Showed()
        {
            TrySendStart();
        }

        private void TrySendStart()
        {
            if (!_event.IsLocked && !_event.IsStartSent())
            {
                TrackOpened();
            }
        }

        private Product GetSegmentedIAP() // Mb Meditator/stategy or something for all events
        {
            var segmentedProduct = _event.SegmentIAP();
            if (segmentedProduct == null)
            {
                return null;
            }

            _iapProvider.AllProducts.TryGetValue(segmentedProduct.ProductId, out var product);

            return product;
        }
    }
}
