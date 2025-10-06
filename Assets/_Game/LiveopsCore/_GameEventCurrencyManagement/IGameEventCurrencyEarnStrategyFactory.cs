namespace _Game.LiveopsCore._GameEventCurrencyManagement
{
    public interface IGameEventCurrencyEarnStrategyFactory
    {
        IGameEventCurrencyEarnStrategy GetEarningStrategy(GameEventBase gameEvent);
    }
}