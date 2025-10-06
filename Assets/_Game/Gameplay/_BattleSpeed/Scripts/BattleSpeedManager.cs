using System.Collections.Generic;
using _Game.Core._GameListenerComposite;

namespace _Game.Gameplay._BattleSpeed.Scripts
{
    public class BattleSpeedManager : IBattleSpeedManager
    {
        public float CurrentSpeedFactor { get; private set; }
        
        private readonly List<IGameSpeedListener> _listeners = new();

        public void Register(IGameSpeedListener listener)
        {
            _listeners.Add(listener);
        }

        public void Unregister(IGameSpeedListener listener)
        {
            _listeners.Remove(listener);
        }
        
        public void SetSpeedFactor(float speedFactor)
        {
            CurrentSpeedFactor = speedFactor;
            
            foreach (var listener in _listeners)
            {
                listener.OnBattleSpeedFactorChanged(speedFactor);
            }
        }
    }
}