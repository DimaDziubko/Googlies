using System;

namespace _Game.Core.UserState._State
{
    public interface IBattleStatisticsReadonly
    {
        int BattlesCompleted { get; }
        event Action CompletedBattlesCountChanged;
    }

    public class BattleStatistics : IBattleStatisticsReadonly
    {
        public int BattlesCompleted;

        public event Action CompletedBattlesCountChanged;

        int IBattleStatisticsReadonly.BattlesCompleted => BattlesCompleted;

        public void AddCompletedBattle()
        {
            BattlesCompleted++;    
            CompletedBattlesCountChanged?.Invoke();
        }
    }
}