using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core.Boosts;
using _Game.Gameplay._Boosts.Scripts;
using Sirenix.OdinInspector;

namespace _Game.Core.Data
{
    public class BoostContainer
    {
        [ShowInInspector] private readonly Dictionary<BoostSource, BoostSubContainer> _boostContainers;

        public BoostContainer()
        {
            _boostContainers = Enum.GetValues(typeof(BoostSource))
                .Cast<BoostSource>()
                .Where(source => source != BoostSource.None)
                .ToDictionary(source => source, source => new BoostSubContainer(source));

            foreach (BoostType type in Enum.GetValues(typeof(BoostType)))
            {
                UpdateTotalBoosts(type);
            }
        }

        public float GetBoostValue(BoostSource source, BoostType type)
        {
            return _boostContainers.TryGetValue(source, out var container) ? container.GetBoostValue(type) : 1f;
        }

        public BoostModel GetBoostModel(BoostSource source, BoostType type)
        {
            return _boostContainers.TryGetValue(source, out var container) ? container.GetBoostModel(type) : null;
        }

        public IEnumerable<BoostModel> GetBoostModels(BoostSource source) =>
            _boostContainers.TryGetValue(source, out var container) ? container.GetBoostModels() : null;

        public void ChangeBoost(BoostSource source, BoostType type, float value)
        {
            if (source == BoostSource.Total || type == BoostType.None) return;

            if (_boostContainers.TryGetValue(source, out var container))
            {
                container.SetBoost(type, value);
                UpdateTotalBoosts(type);
            }
        }

        private void UpdateTotalBoosts(BoostType type)
        {
            BoostSubContainer totalContainer = _boostContainers[BoostSource.Total];

            if (type == BoostType.None) return;

            float totalValue = 1f;

            foreach (var kvp in
                     _boostContainers.Where(kvp => kvp.Key != BoostSource.Total))
            {
                totalValue *= kvp.Value.GetBoostValue(type);
            }

            totalContainer.SetBoost(type, totalValue);
        }
    }

}
