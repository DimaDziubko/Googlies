using System.Collections.Generic;
using System.Linq;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core.Configs.Models._WarriorsConfig;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._UpgradesScreen.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using _Game.Utils;
using _Game.Utils.Extensions;
using UnityEngine;

namespace _Game.UI._StatsPopup._Scripts
{
    public class StatsPopupPresenter : IStatsPopupPresenter
    {
        private readonly IBattleModeUnitDataProvider _unitDataProvider;
        private readonly IIconConfigRepository _commonConfig;
        private readonly IUserContainer _userContainer;
        private ITimelineStateReadonly TimelineState => _userContainer.State.TimelineState;

        public StatsPopupPresenter(
            IBattleModeUnitDataProvider unitDataProvider,
            IConfigRepository configRepository,
            IUserContainer userContainer)
        {
            _unitDataProvider = unitDataProvider;
            
            _commonConfig = configRepository.IconConfigRepository;
            _userContainer = userContainer;
        }

        public string GetNameFor(UnitType type)
        {
            IUnitData data = _unitDataProvider.GetDecoratedUnitData(Faction.Player, type, Skin.Ally);
            return data.Name;
        }

        public string GetStatValue(UnitType type, StatType statType, Faction faction)
        {
            IUnitData data = _unitDataProvider.GetDecoratedUnitData(faction, type, Skin.Ally);
            StatInfo statInfo = data.GetStatInfo(statType); 
            return $"<color=white>{statInfo.Value.ToCompactFormat()}</color><color={Constants.SpecialColors.SPECIAL_BLUE}>(x{statInfo.BoostValue.ToCompactFormat()})</color>";
        }

        public bool CanMovePrevious(UnitType currentType) =>
            FindNextAvailable(currentType, false, out _);

        public bool CanMoveNext(UnitType currentType) =>
            FindNextAvailable(currentType, true, out _);

        public bool FindNextAvailable(UnitType currentType, bool forward, out UnitType nextType)
        {
            List<UnitType> openUnits = TimelineState.OpenUnits
                .OrderBy(unit => unit) 
                .ToList();

            int currentIndex = openUnits.IndexOf(currentType);

            if (currentIndex == -1)
            {
                nextType = currentType;
                return false;
            }

            int newIndex = forward ? currentIndex + 1 : currentIndex - 1;
    
            if (newIndex >= 0 && newIndex < openUnits.Count)
            {
                nextType = openUnits[newIndex];
                return true;
            }

            nextType = currentType;
            return false;
        }

        public string GetTimelineText() => $"Timeline {TimelineState.TimelineId + 1}";

        public Sprite GetIconFor(UnitType type, Faction faction)
        {
            IUnitData data = _unitDataProvider.GetDecoratedUnitData(faction, type, Skin.Ally);
            return data.Icon;
        }

        public Sprite GetStatIcon(StatType damage) =>
            _commonConfig.ForStatIcon(damage);
        
    }
}