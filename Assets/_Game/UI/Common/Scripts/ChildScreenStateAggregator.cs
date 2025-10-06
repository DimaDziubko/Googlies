using System.Collections.Generic;
using System.Linq;
using _Game.UI.Common.Scripts;
using Sirenix.OdinInspector;

namespace _Game.UI._UpgradesScreen.Scripts
{
    public class ScreenStateAggregator
    {
        [ShowInInspector, ReadOnly]
        private readonly Dictionary<IGameScreen, bool> _isReviewed = new();
        [ShowInInspector, ReadOnly]
        private readonly Dictionary<IGameScreen, bool> _needAttention = new();

        [ShowInInspector, ReadOnly]
        public bool IsReviewed => _isReviewed.Values.All(x => x);
        
        [ShowInInspector, ReadOnly]
        public bool NeedAttention => _needAttention.Values.Any(x => x);

        public bool UpdateState(IGameScreen screen)
        {
            UpdateChildState(screen, _isReviewed, screen.IsReviewed);
            UpdateChildState(screen, _needAttention, screen.NeedAttention);

            return NeedAttention;
        }

        private void UpdateChildState(IGameScreen screen, Dictionary<IGameScreen, bool> stateDictionary, bool newState)
        {
            if (stateDictionary.TryGetValue(screen, out bool currentState))
            {
                if (currentState != newState)
                {
                    stateDictionary[screen] = newState;
                }
            }
            else
            {
                stateDictionary.Add(screen, newState);
            }
        }

        public void Cleanup()
        {
            _isReviewed.Clear();
            _needAttention.Clear();
        }
    }
}