using System;

namespace _Game.Core.Navigation.Age
{
    public interface IAgeLoader
    {
        void LoadNextAge(int timelineId, int ageIdx, Action onCompleted);
        void LoadPreviousAge(int timelineId, int ageIdx, Action onCompleted);
    }
}