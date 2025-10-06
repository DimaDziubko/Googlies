using System;
using UnityEngine;

namespace _Game.UI._EvolveScreen.Scripts
{
    public interface IEvolveScreenPresenter
    {
        event Action StateChanged;
        event Action ButtonStateChanged;
        float GetEvolutionPrice();
        string GetTimelineNumber();
        Sprite GetNextAgeIcon();
        Sprite GetCurrentAgeIcon();
        string GetCurrentAgeName();
        string GetNextAgeName();
        void OnEvolveClicked();
        void OnInactiveEvolveClicked();
        bool CanEvolve();
        void OnInfoClicked();
        bool IsNextAgeAffordable();
        void OnScreenClosed();
        void OnScreenOpen();
        void OnScreenDisposed();
        void OnScreenActiveChanged(bool isActive);
        Sprite GetCurrencyIcon();
    }
}