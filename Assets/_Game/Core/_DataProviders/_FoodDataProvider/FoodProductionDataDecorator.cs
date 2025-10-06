using UnityEngine;

namespace _Game.Core._DataProviders._FoodDataProvider
{
    public abstract class FoodProductionDataDecorator : IFoodProductionData
    {
        protected readonly IFoodProductionData _data;

        public FoodProductionDataDecorator(IFoodProductionData data)
        {
            _data = data;
        }

        public Sprite FoodIcon => _data.FoodIcon;
        public virtual float ProductionSpeed => _data.ProductionSpeed;
        public int InitialFoodAmount => _data.InitialFoodAmount;
    }
}