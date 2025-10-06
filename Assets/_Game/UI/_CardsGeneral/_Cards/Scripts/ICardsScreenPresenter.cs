namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public interface ICardsScreenPresenter
    {
        CardsScreen Screen { set; }
        void OnCardScreenOpened();
        void OnCardScreenClosed();
        void OnCardScreenActiveChanged(bool isActive);
    }
}