namespace _Game.UI._CardsGeneral.Scripts
{
    public interface IGeneralCardsScreenPresenter
    {
        GeneralCardsScreen Screen { set; }
        void OnGeneralCardsScreenOpened();
        void OnGeneralCardsScreenClosed();
        void OnScreenDispose();
        void OnScreenActiveChanged(bool isActive);
    }
}