namespace _Game.UI._Shop.Scripts._ShopScr
{
    public interface IShopPresenter
    {
        Shop Shop { set; }
        void OnShopOpened();
        void OnShopClosed();
    }
}