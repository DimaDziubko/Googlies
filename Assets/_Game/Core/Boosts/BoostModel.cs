using System;
using _Game.Gameplay._Boosts.Scripts;
using Sirenix.OdinInspector;

namespace _Game.Core.Boosts
{
    public class BoostModel
    {
        public event Action Changed;
        public event Action<BoostType, float> DetailedChanged;
        public BoostSource Source { get; }
        public BoostType Type { get; }
        
        [ShowInInspector]
        public float Value { get; private set; }

        public BoostModel(BoostSource source, BoostType type, float initialValue = 1f)
        {
            Source = source;
            Type = type;
            Value = initialValue;
        }

        public void UpdateValue(float newValue)
        {
            Value = newValue;
            Changed?.Invoke();
            DetailedChanged?.Invoke(Type, newValue);
        }
    }
}