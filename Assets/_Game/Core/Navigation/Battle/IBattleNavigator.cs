using System;

namespace _Game.Core.Navigation.Battle
{
    public interface IBattleNavigator
    {
        int CurrentBattleIdx { get;}
        bool AllBattlesWon { get;}
        int LastBattleNumber { get;}
        event Action BattleChanged;
        void MoveToPreviousBattle();
        void MoveToNextBattle();
        void OpenNextBattle();
        void ForceMoveToNextBattle();
        bool IsNextBattle();
        bool IsPreviousBattle();
        bool CanMoveToNextBattle();
        void SetAllBattlesWon();
        void InitLevel();
    }
}