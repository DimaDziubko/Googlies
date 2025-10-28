using _Game.UI._EvolveScreen.Scripts;
using System;

namespace _Game.UI._TravelScreen.Scripts
{
    public interface ITravelScreenPresenter
    {
        event Action StateChanged;
        bool CanTravel { get;}
        string GetTravelConditionHint();
        string GetTravelInfo();
        void OnTravelButtonClicked(EvolveScreen evolveScreen);
        string GetTravelText();
        void OnScreenOpen();
        void OnScreenClosed();
        void OnScreenDisposed();
        void OnScreenActiveChanged(bool isActive);
    }
}