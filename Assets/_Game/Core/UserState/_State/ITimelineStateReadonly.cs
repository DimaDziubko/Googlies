using System;
using System.Collections.Generic;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._UpgradesScreen.Scripts;

namespace _Game.Core.UserState._State
{
    public interface ITimelineStateReadonly
    {
        int TimelineId { get; }
        int AgeId { get; }
        int MaxBattle { get; }
        bool AllBattlesWon { get; }
        int Level { get; }
        int MaxWave { get; }
        List<UnitType> OpenUnits { get; }
        event Action<UnitType> OpenedUnit;
        event Action NextAgeOpened;
        event Action NextTimelineOpened;
        event Action NextBattleOpened;
        event Action LastBattleWon;
        event Action<UpgradeItemType, int> UpgradeItemLevelChanged;
        event Action PreviousAgeOpened;
        event Action PreviousTimelineOpened;
        event Action<int> LevelUp;
        event Action<int> OnTrackWave;

        int GetUpgradeLevel(UpgradeItemType type);

        int TimelineNumber { get; }
        int AgeNumber { get; }
        int BattleNumber { get; }

        public void TrackWave(int wave);
    }
}