using _Game.Gameplay._Units.Scripts;
using _Game.UI.UpgradesAndEvolution.Scripts;
using UnityEngine;

namespace _Game.UI._StatsPopup._Scripts
{
    public interface IStatsPopupPresenter
    {
        string GetNameFor(UnitType type);
        Sprite GetIconFor(UnitType type, Faction faction);
        Sprite GetStatIcon(StatType damage);
        string GetStatValue(UnitType type, StatType health, Faction faction);
        bool CanMovePrevious(UnitType currentType);
        bool CanMoveNext(UnitType currentType);
        bool FindNextAvailable(UnitType currentType, bool forward, out UnitType nextType);
        string GetTimelineText();
    }
}