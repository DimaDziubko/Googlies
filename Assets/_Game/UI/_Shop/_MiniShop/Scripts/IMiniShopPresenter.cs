namespace _Game.UI._Shop._MiniShop.Scripts
{
    public interface IMiniShopPresenter
    {
        MiniShop MiniShop { set; }
        void OnMiniShopOpened();
        void OnMiniShopClosed();
    }
}