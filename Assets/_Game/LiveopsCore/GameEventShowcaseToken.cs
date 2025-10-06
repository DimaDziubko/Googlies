using System;
using _Game.LiveopsCore._Enums;

namespace _Game.LiveopsCore
{
    public class GameEventShowcaseToken
    {
        public event Action Requested;
        public event Action Shown;
        public event Action Closed;

        private readonly GameEventShowcaseSavegamne _save;
        public bool IsRequested { get; private set; }
        public bool IsClosed { get; private set; }

        
        public int ShowOrder => _save.ShowOrder;
        public ShowcaseCondition ShowcaseCondition => _save.ShowcaseCondition;
        public bool IsShown => _save.IsShown;
        
        public GameEventShowcaseToken(GameEventShowcaseSavegamne save)
        {
            _save = save;
        }

        public void SetShown(bool isShown)
        {
            _save.SetShown(isShown);
        }
        
        public void Request()
        {
            if (IsRequested) return;
            IsRequested = true;
            Requested?.Invoke();
        }

        public void MarkShown()
        {
            if (IsShown) return;
            _save.SetShown(true);
            Shown?.Invoke();
        }

        public void MarkClosed()
        {
            if (IsClosed) return;
            IsClosed = true;
            Closed?.Invoke();
        }
    }
}