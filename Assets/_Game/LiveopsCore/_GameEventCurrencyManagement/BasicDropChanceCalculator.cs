using _Game.Core.Services.Random;
using UnityEngine;

namespace _Game.LiveopsCore._GameEventCurrencyManagement
{
    public class BasicDropChanceCalculator : IUnitDropChanceCalculator
    {
        private readonly IRandomService _random;

        public BasicDropChanceCalculator(IRandomService random)
        {
            _random = random;
        }
        
        public bool ShouldDrop(UnitLootDropSettingsWithDropChance settings)
        {
            return _random.GetValue() < Mathf.Clamp01(settings.DropChance);
        }
    }
}