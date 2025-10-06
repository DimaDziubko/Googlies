using _Game.Core._Logger;
using _Game.UI._MainMenu.Scripts;
using _Game.UI._Shop.Scripts._ShopScr;
using _Game.Utils.Disposable;
using Cysharp.Threading.Tasks;
using UnityUtils;

namespace _Game.UI._MainMenu.State
{
    public class ShopState : ILocalState
    {
        private readonly MainMenu _mainMenu;
        private readonly IShopProvider _provider;
        private readonly IMyLogger _logger;

        private Disposable<Shop> _shop;

        public ShopState(
            MainMenu mainMenu,
            IShopProvider provider,
            IMyLogger logger
            )
        {
            _provider = provider;
            _logger = logger;
            _mainMenu = mainMenu;
        }
        public async UniTask InitializeAsync() => _shop = await _provider.Load();
        
        public void SetActive(bool isActive)
        {
            if (_shop != null) 
                _shop.Value.SetActive(isActive);
        }
        
        public void Enter()
        {
            _mainMenu.SetButtonHighlighted(MenuButtonType.Shop, true);

            if (_shop != null)
            {
                _shop.Value.Show();
            }

        }

        public void Exit()
        {
            if (_shop?.Value.OrNull() != null)
            {
                _shop.Value.Hide();
            }
            else
            {
                _logger.Log("Exit called but _shop is null", DebugStatus.Warning);
            }

            _mainMenu.SetButtonHighlighted(MenuButtonType.Shop, false);
        }

        public void Cleanup()
        {
            _provider.Dispose();
            _mainMenu.SetButtonHighlighted(MenuButtonType.Shop, false);
        }
    }
}