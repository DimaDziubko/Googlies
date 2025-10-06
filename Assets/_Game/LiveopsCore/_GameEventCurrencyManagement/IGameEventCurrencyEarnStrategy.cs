namespace _Game.LiveopsCore._GameEventCurrencyManagement
{
    public interface IGameEventCurrencyEarnStrategy
    {
        public void Execute();
        public void UnExecute();
    }
}