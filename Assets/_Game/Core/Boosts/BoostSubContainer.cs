using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Boosts;
using _Game.Gameplay._Boosts.Scripts;
using Sirenix.OdinInspector;

namespace _Game.Core.Data
{
    public class BoostSubContainer
    {
        public BoostSource Source { get; }
        
        [ShowInInspector]
        private readonly Dictionary<BoostType, BoostModel> _boosts;

        public BoostSubContainer(BoostSource source)
        {
            Source = source;
            _boosts = Enum.GetValues(typeof(BoostType))
                .Cast<BoostType>()
                .Where(type => type != BoostType.None)
                .ToDictionary(type => type, type => new BoostModel(source, type));
        }
        
        public float GetBoostValue(BoostType type) => 
            _boosts.TryGetValue(type, out var boost) ? boost.Value : 1f;
        
        public BoostModel GetBoostModel(BoostType type) => 
            _boosts.TryGetValue(type, out var boost) ? boost : null;

        public IEnumerable<BoostModel> GetBoostModels() => _boosts.Values;

        public void SetBoost(BoostType type, float value) 
        {
            if (_boosts.TryGetValue(type, out var boost))
            {
                boost.UpdateValue(value);
            }
        }
    }
}