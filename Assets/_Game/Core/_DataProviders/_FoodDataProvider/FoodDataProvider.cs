using _Game.Core._DataPresenters._UpgradeItem;
using _Game.Core.Boosts;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Configs.Repositories.Economy;
using _Game.Core.Data;
using _Game.Gameplay._Boosts.Scripts;
using _Game.UI._UpgradesScreen.Scripts;

namespace _Game.Core._DataProviders._FoodDataProvider
{
    public class BattleFoodProductionProvider : IFoodProductionProvider
    {
        private readonly IIconConfigRepository _commonConfig;
        private readonly IEconomyConfigRepository _economyConfig;
        private readonly UpgradeItemContainer _upgradeItemContainer;
        private readonly BoostContainer _boosts;
        
        public BattleFoodProductionProvider(
            IConfigRepository configRepository,
            BoostContainer boosts,
            UpgradeItemContainer upgradeItemContainer)
        {
            _commonConfig = configRepository.IconConfigRepository;
            _economyConfig = configRepository.EconomyConfigRepository;
            _boosts = boosts;
            _upgradeItemContainer = upgradeItemContainer;
        }


        public IFoodProductionData GetData()
        {
            var data = new FoodProductionData()
            {
                FoodIcon = _commonConfig.FoodIcon(),
                ProductionSpeed = _upgradeItemContainer.GetItem(UpgradeItemType.FoodProduction).Value,
                InitialFoodAmount = _economyConfig.GetInitialFoodAmount()
            };

            FoodProductionBoostDecorator productionBoostDecorator 
                = new FoodProductionBoostDecorator(data, _boosts.GetBoostValue(BoostSource.Total, BoostType.FoodProduction));
            
            return productionBoostDecorator;
        }
    }
}