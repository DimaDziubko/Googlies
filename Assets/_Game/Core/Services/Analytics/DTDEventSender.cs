using _Game.Core._Logger;
using _Game.Utils._Dtd;
using DevToDev.Analytics;

namespace _Game.Core.Services.Analytics
{
    public class DTDEventSender : IAnalyticsEventSender
    {
        private readonly IMyLogger _logger;

        public DTDEventSender(IMyLogger logger)
        {
            _logger = logger;
        }
        
        public void CustomEvent(string eventName)
        {
            DTDAnalytics.CustomEvent(eventName);
            
            _logger.Log($"Dev2Dev Event {eventName}");
        }

        public void CustomEvent(string eventName, DTDCustomEventParameters parameters)
        {
            DTDAnalytics.CustomEvent(eventName, parameters);
            
            _logger.Log($"Dev2Dev Event With Params {eventName}");
            _logger.Log($"Dev2Dev| Params: {parameters.ToLogString()}");
        }
        
        public void SetUserData(string key, string value) => DTDUserCard.Set(key: key, value: value);

        public void SetUserData(string key, bool value) => DTDUserCard.Set(key: key, value: value);

        public void SetUserData(string key, long value) => DTDUserCard.Set(key: key, value: value);

        public void SetUserData(string key, double value) => DTDUserCard.Set(key: key, value: value);
        public void Tutorial(int step) => DTDAnalytics.Tutorial(step);
        public void LevelUp(int level) => DTDAnalytics.LevelUp(level);
        public void SetCurrentLevel(int level) => DTDAnalytics.SetCurrentLevel(level);
        public void RealCurrencyPayment(string orderId, double price, string productId, string currencyCode) => 
            DTDAnalytics.RealCurrencyPayment(orderId, price, productId, currencyCode);
        public void AdImpression(string networkName, double revenue, string placement, string adUnitIdentifier) => 
            DTDAnalytics.AdImpression( networkName, revenue, placement, adUnitIdentifier);
    }
}