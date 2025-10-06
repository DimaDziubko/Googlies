using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._Buyer;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.UI.Common.Scripts;
using DevToDev.Analytics;

namespace _Game.Core.Services.Analytics
{
    public class CardsTracker
    {
        private readonly IUserContainer _userContainer;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        
        private readonly IProductBuyer _productBuyer;
        private readonly IAnalyticsEventSender _sender;

        public CardsTracker(
            IUserContainer userContainer,
            IProductBuyer productBuyer,
            IAnalyticsEventSender sender)
        {
            _sender = sender;
            _productBuyer = productBuyer;
            _userContainer = userContainer; 
        }

        public void Initialize()
        {
            _productBuyer.ProductBought += OnProductBought;
        }

        public void Dispose()
        {
            _productBuyer.ProductBought -= OnProductBought;
        }

        private void OnProductBought(IProduct product)
        {
            if (product is CardsBundle cardsBundle)
            {
                var parameters = new DTDCustomEventParameters();
                
                parameters.Add("Gems", cardsBundle.Price[0].Amount);
                parameters.Add("TimelineID", TimelineState.TimelineNumber);
                parameters.Add("AgeID", TimelineState.AgeNumber);
                parameters.Add("BattleID", TimelineState.BattleNumber);
                parameters.Add("Source", cardsBundle.Source.ToString());
                
                _sender.CustomEvent("cards_purchased", parameters);
            }
        }
    }
}