using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core.Boosts;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Data;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._Boosts.Scripts;

namespace _Game.Core._DataProviders._FoodDataProvider
{
    public class ZombieRushFoodProductionProvider : IFoodProductionProvider
    {
        private readonly IIconConfigRepository _commonConfig;
        private readonly IUserContainer _userContainer;
        private readonly BoostContainer _boosts;
        private readonly IDungeonModel _dungeon;

        public ZombieRushFoodProductionProvider(
            IConfigRepository configRepository,
            BoostContainer boosts,
            IUserContainer userContainer,
            IDungeonModelFactory dungeonModelFactory)
        {
            _commonConfig = configRepository.IconConfigRepository;
            _boosts = boosts;
            _userContainer = userContainer;
            _dungeon = dungeonModelFactory.GetModel(DungeonType.ZombieRush);
        }
        
        public IFoodProductionData GetData()
        {
            var data = new FoodProductionData()
            {
                FoodIcon = _commonConfig.FoodIcon(),
                ProductionSpeed = _dungeon.FoodProductionSpeed,
                InitialFoodAmount = _dungeon.InitialFoodAmount,
            };

            FoodProductionBoostDecorator productionBoostDecorator 
                = new FoodProductionBoostDecorator(data, _boosts.GetBoostValue(BoostSource.Total, BoostType.FoodProduction));
            
            return productionBoostDecorator;
        }
    }
}