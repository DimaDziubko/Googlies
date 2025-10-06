using UnityEngine;

namespace _Game.Core._DataProviders._FoodDataProvider
{
    public class FoodProductionData : IFoodProductionData
    {
        public Sprite FoodIcon { get; set; }
        public float ProductionSpeed { get; set; }
        public int InitialFoodAmount { get; set; }
    }
}