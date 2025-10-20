using System;

namespace _Game.UI._TravelScreen.Scripts
{
    public interface ITravelScreenPresenter
    {
        event Action StateChanged;
        bool CanTravel { get;}
        string GetTravelConditionHint();
        string GetTravelInfo();
        void OnTravelButtonClicked();
        string GetTravelText();
    }
}