namespace _Game.Core._DataProviders._FoodDataProvider
{
    public class FoodProductionBoostDecorator : FoodProductionDataDecorator
    {
        private readonly float _productionSpeedMultiplier;

        public FoodProductionBoostDecorator(IFoodProductionData data, float productionSpeedMultiplier) : base(data)
        {
            _productionSpeedMultiplier = productionSpeedMultiplier;
        }

        public override float ProductionSpeed => _data.ProductionSpeed * _productionSpeedMultiplier;
    }
}