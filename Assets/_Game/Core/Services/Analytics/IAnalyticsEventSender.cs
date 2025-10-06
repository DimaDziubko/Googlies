using DevToDev.Analytics;

namespace _Game.Core.Services.Analytics
{
    public interface IAnalyticsEventSender
    {
        void CustomEvent(string eventName, DTDCustomEventParameters parameters);
        void CustomEvent(string eventName);
        void SetUserData(string key, string value);
        void SetUserData(string key, bool value);
        void SetUserData(string key, long value);
        void SetUserData(string key, double value);
        void Tutorial(int step);
        void LevelUp(int level);
        void SetCurrentLevel(int level);
        void RealCurrencyPayment(string orderId, double price, string productId, string currencyCode);
        void AdImpression(string networkName, double revenue, string placement, string adUnitIdentifier);
    }
}