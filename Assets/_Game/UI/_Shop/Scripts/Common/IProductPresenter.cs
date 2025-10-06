using _Game.UI._Shop.Scripts._ShopScr;

namespace _Game.UI._Shop.Scripts.Common
{
    public interface IProductPresenter
    {
        public void Initialize();
        public ShopItemView View { get; }
    }
}