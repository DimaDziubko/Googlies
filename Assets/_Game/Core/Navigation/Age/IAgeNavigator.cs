using System;

namespace _Game.Core.Navigation.Age
{
    public interface IAgeNavigator
    {
        public event Action AgeChanged;
        int CurrentIdx { get;}
        bool IsNextAge();
        void MoveToNextAge();
        int GetTotalAgesCount();
        void MoveToPreviousAge();
        bool IsPreviousAge();
        bool IsAllBattlesWon();
    }
}