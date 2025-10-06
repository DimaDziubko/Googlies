using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Gameplay._Units.Scripts;
using _Game.UI._UpgradesScreen.Scripts;

namespace _Game.Core.UserState._State
{
    public class TimelineState : ITimelineStateReadonly
    {
        public int TimelineId;
        public int MaxBattle;
        public bool AllBattlesWon;
        public int Level;
        public int MaxWave;

        public int AgeId;
        public List<UpgradeItem> Upgrades;
        public List<UnitType> OpenUnits;

        public event Action<UnitType> OpenedUnit;
        public event Action NextAgeOpened;
        public event Action PreviousAgeOpened;
        public event Action NextBattleOpened;
        public event Action NextTimelineOpened;
        public event Action PreviousTimelineOpened;
        public event Action<UpgradeItemType, int> UpgradeItemLevelChanged;
        public event Action LastBattleWon;
        public event Action<int> LevelUp;
        public event Action<int> OnTrackWave;

        int ITimelineStateReadonly.TimelineId => TimelineId;

        int ITimelineStateReadonly.AgeId => AgeId;
        int ITimelineStateReadonly.MaxBattle => MaxBattle;

        bool ITimelineStateReadonly.AllBattlesWon => AllBattlesWon;

        List<UnitType> ITimelineStateReadonly.OpenUnits => OpenUnits;
        int ITimelineStateReadonly.Level => Level;
        int ITimelineStateReadonly.MaxWave => MaxWave;

        public int TimelineNumber => TimelineId + 1;

        public int AgeNumber => AgeId + 1;

        public int BattleNumber => MaxBattle + 1;

        public void OpenUnit(in UnitType type)
        {
            OpenUnits.Add(type);
            OpenedUnit?.Invoke(type);
        }

        public void OpenNewAge(bool isNext = true)
        {
            foreach (var upgrade in Upgrades)
            {
                upgrade.Reset();
            }

            AllBattlesWon = false;
            MaxBattle = 0;
            MaxWave = 0;
            OpenUnits = new List<UnitType>() { UnitType.Light };

            if (isNext)
            {
                AgeId++;
                NextAgeOpened?.Invoke();
            }
            else
            {
                AgeId--;
                if (AgeId < 0) AgeId = 0;
                PreviousAgeOpened?.Invoke();
            }
        }

        public void OpenNewTimeline(bool isNext = true)
        {
            AgeId = 0;

            foreach (var upgrade in Upgrades)
            {
                upgrade.Reset();
            }

            AllBattlesWon = false;
            MaxBattle = 0;
            MaxWave = 0;
            OpenUnits = new List<UnitType>() { UnitType.Light };

            if (isNext)
            {
                TimelineId++;
                NextTimelineOpened?.Invoke();
            }
            else
            {
                TimelineId--;
                if (TimelineId < 0) TimelineId = 0;
                PreviousTimelineOpened?.Invoke();
            }
        }

        public void OpenNextBattle(int nextBattle)
        {
            MaxBattle = nextBattle;
            CalculateLevel(nextBattle);
            NextBattleOpened?.Invoke();
            MaxWave = 0;
        }

        public void SetAllBattlesWon(bool allBattlesWon)
        {
            //var lastBattleIndex = _config.TimelineConfigRepository.LastBattleIndex(); //Inject == null //TODO fix
            var lastBattleIndex = 5;
            AllBattlesWon = allBattlesWon;
            MaxBattle = lastBattleIndex;
            LastBattleWon?.Invoke();
        }

        public int GetUpgradeLevel(UpgradeItemType type)
        {
            UpgradeItem upgrade = Upgrades.FirstOrDefault(u => u.Type == type);
            return upgrade?.Level ?? 0;
        }

        public void UpgradeItemLevelUp(UpgradeItemType type)
        {
            var upgrade = Upgrades.Find(u => u.Type == type);
            if (upgrade != null)
            {
                upgrade.Level++;
                UpgradeItemLevelChanged?.Invoke(type, upgrade.Level);
            }
        }

        public void InitLevel() => CalculateLevel(MaxBattle);
        public void TrackWave(int wave)
        {
            if (MaxBattle < AgeId) return;
            if (wave > MaxWave)
            {
                MaxWave = wave;
                OnTrackWave?.Invoke(MaxWave);
            }
        }

        private void CalculateLevel(int nextBattleId)
        {
            int newUniqBattleId = TimelineId * 6 + nextBattleId + 1;
            if (Level < newUniqBattleId)
            {
                Level = newUniqBattleId;
                LevelUp?.Invoke(Level);
            }
        }
    }
}