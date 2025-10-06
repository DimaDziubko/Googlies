using UnityEngine;

namespace _Game.Core._DataProviders._FoodDataProvider
{
    public interface IFoodProductionData
    {
        Sprite FoodIcon { get; }
        public float ProductionSpeed { get; }
        public int InitialFoodAmount { get; }
    }
}