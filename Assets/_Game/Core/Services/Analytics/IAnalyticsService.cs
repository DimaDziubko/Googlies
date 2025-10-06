namespace _Game.Core.Services.Analytics
{
    public interface IAnalyticsService
    {
        void SendEvent(string eventName);
    }
}