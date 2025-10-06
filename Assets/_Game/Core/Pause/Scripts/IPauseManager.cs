namespace _Game.Core.Pause.Scripts
{
    public interface IPauseManager 
    {
        bool IsPaused { get; }
        void AddHandler(IPauseHandler handler);
        public void RemoveHandler(IPauseHandler handler);
        void SetPaused(bool isPaused);
    }
}