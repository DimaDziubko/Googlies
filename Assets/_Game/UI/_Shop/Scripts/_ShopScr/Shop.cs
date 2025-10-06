using _Game.Core._Logger;
using _Game.UI.Factory;
using UnityEngine;

namespace _Game.UI._Shop.Scripts._ShopScr
{
    [RequireComponent(typeof(Canvas))]
    public class Shop : MonoBehaviour
    {
        public ShopItemsContainer Container => _container;

        [SerializeField] private Canvas _canvas;
        [SerializeField] private ShopItemsContainer _container;
        
        private IShopPresenter _shopPresenter;
        
        public void Construct(
            Camera uICamera,
            IUIFactory uiFactory,
            IShopPresenter shopPresenter,
            IMyLogger logger)
        {
            _canvas.worldCamera = uICamera;
            _shopPresenter = shopPresenter;
            _container.Construct(uiFactory, logger);
            shopPresenter.Shop = this;
            _canvas.enabled = false;
        }

        public void Show()
        {
            _shopPresenter.OnShopOpened();
            _canvas.enabled = true;
        }

        public void Hide()
        {
            _canvas.enabled = false;
            _shopPresenter.OnShopClosed();
            _container.Cleanup();
        }

        public void SetActive(bool isActive) => 
            gameObject.SetActive(isActive);
    }
}