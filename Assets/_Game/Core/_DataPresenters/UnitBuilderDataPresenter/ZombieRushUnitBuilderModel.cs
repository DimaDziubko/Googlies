using System.Collections.Generic;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Gameplay._UnitBuilder.Scripts;

namespace _Game.Core._DataPresenters.UnitBuilderDataPresenter
{
    public class ZombieRushUnitBuilderModel : IUnitBuilderModel
    {
        private readonly IIconConfigRepository _config;
        private readonly IZombieRushModeUnitDataProvider _provider;

        public ZombieRushUnitBuilderModel(
            IConfigRepository configRepository,
            IZombieRushModeUnitDataProvider provider)
        {
            _config = configRepository.IconConfigRepository;
            _provider = provider;
        }
        
        public IEnumerable<UnitBuilderBtnModel> GetBtnModels()
        {
            foreach (var unitData in _provider.GetAllPlayerUnits())
            {
                yield return new UnitBuilderBtnModel(
                    unitData.Type,
                    _config.FoodIcon(),
                    unitData.Icon,
                    unitData.FoodPrice,
                    true);
            }
        }
    }
}