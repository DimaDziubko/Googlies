using System;
using System.Collections.Generic;
using System.Linq;
using _Game.Core._GameInitializer;
using _Game.Core._Logger;
using _Game.Core.Services._AdsGemsPackService;
using _Game.Core.Services._FreeGemsPackService;
using _Game.Core.Services.IAP;
using _Game.Core.Services.IGPService;
using _Game.Core.UserState._State;
using _Game.UI._Shop.Scripts._AdsGemsPack;
using _Game.UI._Shop.Scripts._CoinBundles;
using _Game.UI._Shop.Scripts._FreeGemsPack;
using _Game.UI._Shop.Scripts._GemsBundle;
using _Game.UI._Shop.Scripts._ProfitOffer;
using _Game.UI._Shop.Scripts._SpeedOffer;
using _Game.UI.Common.Scripts;
using _Game.UI.Global;
using Sirenix.OdinInspector;
using CurrencyType = _Game.Core.UserState._State.CurrencyType;

namespace _Game.UI._Shop.Scripts._ShopScr
{
    public class ShopPresenter :
        IShopPresenter,
        IDisposable,
        IGameScreenEvents<IShopScreen>,
        IGameScreenListener<IMenuScreen>,
        IShopScreen
    {

        private const int REQUEST_ATTENTION_LIMIT = 1;
        public event Action<IShopScreen> ScreenOpened;
        public event Action<IShopScreen> InfoChanged;
        public event Action<IShopScreen> RequiresAttention;
        public event Action<IShopScreen> ScreenClosed;
        public event Action<IShopScreen, bool> ActiveChanged;
        public event Action<IShopScreen> ScreenDisposed;

        [ShowInInspector, ReadOnly] public bool IsReviewed { get; private set; }

        [ShowInInspector, ReadOnly]
        public bool NeedAttention => (_igpService.CoinsBundles.Any(bundle => _bank.IsEnough(bundle.Price)) ||
                                                                            _adsGemsPackService.GetAdsGemsPacks()
                                                                                .Any(pack => pack.Value.IsReady) ||
                                                                            _freeGemsPackService.GetFreeGemsPacks()
                                                                                .Any(pack => pack.Value.IsReady)) &&
                                                                            _reviewCounter < REQUEST_ATTENTION_LIMIT;

        private int _reviewCounter;

        public string Info => "<style=Golden>Shop</style>";

        private readonly IIGPService _igpService;
        private readonly IIAPService _iapService;
        private readonly IAdsGemsPackService _adsGemsPackService;
        private readonly IFreeGemsPackService _freeGemsPackService;

        private readonly IGameInitializer _gameInitializer;
        private readonly IUINotifier _uiNotifier;
        private readonly IMyLogger _logger;

        private readonly ShopItemManager<CoinsBundle, CoinsBundleView, CoinsBundlePresenter> _coinsBundleManager;
        private readonly ShopItemManager<GemsBundle, GemsBundleView, GemsBundlePresenter> _gemsBundleManager;
        private readonly ShopItemManager<ProfitOfferModel, ProfitOfferView, ProfitOfferPresenter> _profitOfferManager;
        private readonly ShopItemManager<AdsGemsPack, AdsGemsPackView, AdsGemsPackPresenter> _adsGemsPackManager;
        private readonly ShopItemManager<FreeGemsPack, FreeGemsPackView, FreeGemsPackPresenter> _freeGemsPackManager;

        public Shop Shop { get; set; }

        private readonly CurrencyBank _bank;
        private CurrencyCell GemsCell { get; set; }

        public ShopPresenter(
            IIGPService igpService,
            IIAPService iapService,
            IGameInitializer gameInitializer,
            IAdsGemsPackService adsGemsPackService,
            IFreeGemsPackService freeGemsPackService,
            IMyLogger logger,
            IUINotifier uiNotifier,
            CurrencyBank bank,
            CoinsBundlePresenter.Factory coinsBundlePresenterFactory,
            GemsBundlePresenter.Factory gemsBundlePresenterFactory,
            SpeedOfferPresenter.Factory speedOfferPresenterFactory,
            ProfitOfferPresenter.Factory profitOfferPresenterFactory,
            AdsGemsPackPresenter.Factory adsGemsPackPresenterFactory,
            FreeGemsPackPresenter.Factory freeGemsPackPresenterFactory)
        {

            _bank = bank;
            _igpService = igpService;
            _iapService = iapService;
            _adsGemsPackService = adsGemsPackService;
            _freeGemsPackService = freeGemsPackService;
            _logger = logger;
            gameInitializer.OnPostInitialization += Init;
            _gameInitializer = gameInitializer;

            _uiNotifier = uiNotifier;

            _uiNotifier.RegisterScreen(this, this);

            _coinsBundleManager = new ShopItemManager<CoinsBundle, CoinsBundleView, CoinsBundlePresenter>(
                null,
                coinsBundlePresenterFactory
            );
            
            _gemsBundleManager = new ShopItemManager<GemsBundle, GemsBundleView, GemsBundlePresenter>(
                null,
                gemsBundlePresenterFactory
            );

            _profitOfferManager = new ShopItemManager<ProfitOfferModel, ProfitOfferView, ProfitOfferPresenter>(
                null,
                profitOfferPresenterFactory
            );

            _adsGemsPackManager = new ShopItemManager<AdsGemsPack, AdsGemsPackView, AdsGemsPackPresenter>(
                null,
                adsGemsPackPresenterFactory
            );

            _freeGemsPackManager = new ShopItemManager<FreeGemsPack, FreeGemsPackView, FreeGemsPackPresenter>(
                null,
                freeGemsPackPresenterFactory
            );
        }

        private void Init()
        {
            GemsCell = _bank.GetCell(CurrencyType.Gems);
            
            GemsCell.OnAmountAdded += OnStateChanged;

            IsReviewed = !NeedAttention;

            OnStateChanged(0);
        }

        private void OnStateChanged(double _)
        {
            if (NeedAttention)
            {
                IsReviewed = false;
                RequiresAttention?.Invoke(this);
            }
        }

        void IDisposable.Dispose()
        {
            _uiNotifier.UnregisterScreen(this);
            _gameInitializer.OnPostInitialization -= Init;
            GemsCell.OnAmountAdded -= OnStateChanged;
            ScreenDisposed?.Invoke(this);
        }
        
        void IShopPresenter.OnShopOpened()
        {
            if (Shop != null)
            {
                _igpService.UpdateProducts();
                _iapService.UpdateProducts();
                UpdateAllItems();
                IsReviewed = true;
                ScreenOpened?.Invoke(this);
                _reviewCounter++;
            }
        }

        void IShopPresenter.OnShopClosed()
        {
            _coinsBundleManager.ClearItems();
            _gemsBundleManager.ClearItems();
            _profitOfferManager.ClearItems();
            _adsGemsPackManager.ClearItems();
            _freeGemsPackManager.ClearItems();
            ScreenClosed?.Invoke(this);
        }

        private void UpdateAllItems()
        {
            if (Shop == null) return;

            _coinsBundleManager.SetShopContainer(Shop.Container);
            _gemsBundleManager.SetShopContainer(Shop.Container);
            _profitOfferManager.SetShopContainer(Shop.Container);
            _adsGemsPackManager.SetShopContainer(Shop.Container);
            _freeGemsPackManager.SetShopContainer(Shop.Container);

            UpdateItems(_iapService.ProfitOfferContainer.GetAll(), AddProfitOffer);
            UpdateItems(_igpService.CoinsBundles, AddCoinsBundle);
            UpdateItems(_iapService.GemsBundleContainer.GetAll(), AddGemsBundle);
            UpdateItems(_adsGemsPackService.GetAdsGemsPacks().Values.ToList(), AddAdsGemsPacks);
            UpdateItems(_freeGemsPackService.GetFreeGemsPacks().Values.ToList(), AddFreeGemsPacks);
            UpdateDecorationElements();
        }

        private void UpdateItems<T>(IEnumerable<T> items, Action<T> addItemAction)
        {
            if (items == null) return;

            foreach (var item in items)
            {
                addItemAction(item);
            }
        }

        private void UpdateDecorationElements() =>
            Shop.Container.UpdateDecorationElements();

        private void AddCoinsBundle(CoinsBundle bundle) =>
            _coinsBundleManager.AddItem(bundle, ShopSubContainerType.ForCoins);
        
        private void AddGemsBundle(GemsBundle bundle) =>
            _gemsBundleManager.AddItem(bundle, ShopSubContainerType.ForGems);
        
        private void AddProfitOffer(ProfitOfferModel offer) =>
            _profitOfferManager.AddItem(offer, ShopSubContainerType.ForOffers);

        private void AddAdsGemsPacks(AdsGemsPack adsGemsPack) =>
            _adsGemsPackManager.AddItem(adsGemsPack, ShopSubContainerType.ForGems);

        private void AddFreeGemsPacks(FreeGemsPack freeGemsPack) =>
            _freeGemsPackManager.AddItem(freeGemsPack, ShopSubContainerType.ForGems);
        

        [Button]
        public void OnRequiredAttention()
        {
            OnStateChanged(0);
        }

        void IGameScreenListener<IMenuScreen>.OnScreenOpened(IMenuScreen screen)
        {
            if (NeedAttention)
            {
                IsReviewed = false;
                RequiresAttention?.Invoke(this);
            }
        }

        void IGameScreenListener<IMenuScreen>.OnInfoChanged(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnRequiresAttention(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnScreenClosed(IMenuScreen screen) { }
        void IGameScreenListener<IMenuScreen>.OnScreenActiveChanged(IMenuScreen screen, bool isActive) { }
        void IGameScreenListener<IMenuScreen>.OnScreenDisposed(IMenuScreen screen) { }
    }

}