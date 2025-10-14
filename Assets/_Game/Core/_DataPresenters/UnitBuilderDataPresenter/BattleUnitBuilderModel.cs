using System.Collections.Generic;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._UnitBuilder.Scripts;

namespace _Game.Core._DataPresenters.UnitBuilderDataPresenter
{
    public class BattleUnitBuilderModel : IUnitBuilderModel
    {
        private readonly IIconConfigRepository _config;
        private readonly IBattleModeUnitDataProvider _provider;
        private readonly IUserContainer _userContainer;

        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;
        
        public BattleUnitBuilderModel(
            IConfigRepository configRepository,
            IBattleModeUnitDataProvider provider,
            IUserContainer userContainer)
        {
            _config = configRepository.IconConfigRepository;
            _provider = provider;
            _userContainer = userContainer;
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
                    TimelineState.OpenUnits.Contains(unitData.Type),
                    _config.GetMeleeOrRangedIcon(unitData.IsWeaponMelee)
                    );
            }
        }
    }
}