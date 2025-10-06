using System.Collections.Generic;

namespace _Game.Core.Pause.Scripts
{
    public sealed class PauseManager : IPauseHandler, IPauseManager
    {
        private readonly List<IPauseHandler> _handlers = new();
        public bool IsPaused { get; private set; }
        
        public void AddHandler(IPauseHandler handler)
        {
            _handlers.Add(handler);
        }

        public void RemoveHandler(IPauseHandler handler)
        {
            _handlers.Remove(handler);
        }
        
        public void SetPaused(bool isPaused)
        {
            IsPaused = isPaused;
            foreach (var handler in _handlers)
            {
                handler.SetPaused(isPaused);
            }
        }
    }
}