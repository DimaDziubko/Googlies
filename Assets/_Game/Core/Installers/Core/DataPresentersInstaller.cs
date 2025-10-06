using _Game.Core._DataPresenters._UpgradeItem;
using _Game.Core.Services.IAP;
using _Game.Core.Services.IGPService;
using _Game.UI._Shop._MiniShop.Scripts;
using _Game.UI._Shop.Scripts._AdsGemsPack;
using _Game.UI._Shop.Scripts._CoinBundles;
using _Game.UI._Shop.Scripts._FreeGemsPack;
using _Game.UI._Shop.Scripts._GemsBundle;
using _Game.UI._Shop.Scripts._ProfitOffer;
using _Game.UI._Shop.Scripts._ShopScr;
using _Game.UI._Shop.Scripts._SpeedOffer;
using _Game.UI._UpgradesScreen.Scripts;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class DataPresentersInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindCoinsBundlePresenterFactory();
            BindGemsBundlePresenterFactory();
            BindSpeedOfferPresenterFactory();
            BindProfitOfferPresenterFactory();
            BindAdsGemsPackPresenterFactory();
            BindFreeGemsPackPresenterFactory();
            
            BindUpgradeItemPresenterFactory();
            BindUnitUpgradePresenterFactory();
            
            BindShopPresenter();
            BindMiniShopPresenter();
        }

        private void BindUnitUpgradePresenterFactory() =>
            Container
                .BindFactory<UnitUpgrade, UnitUpgradeView, UnitUpgradePresenter, UnitUpgradePresenter.Factory>()
                .AsSingle()
                .NonLazy();

        private void BindUpgradeItemPresenterFactory() =>
            Container
                .BindFactory<UpgradeItemModel, UpgradeItemView, UpgradeItemPresenter, UpgradeItemPresenter.Factory>()
                .AsSingle()
                .NonLazy();
        private void BindCoinsBundlePresenterFactory() =>
            Container
                .BindFactory<CoinsBundle, CoinsBundleView, CoinsBundlePresenter, CoinsBundlePresenter.Factory>()
                .AsSingle()
                .NonLazy();
        
        private void BindGemsBundlePresenterFactory() =>
            Container
                .BindFactory<GemsBundle, GemsBundleView, GemsBundlePresenter, GemsBundlePresenter.Factory>()
                .AsSingle()
                .NonLazy();
        
        private void BindSpeedOfferPresenterFactory() =>
            Container
                .BindFactory<SpeedOffer, SpeedOfferView, SpeedOfferPresenter, SpeedOfferPresenter.Factory>()
                .AsSingle()
                .NonLazy();
        
        private void BindProfitOfferPresenterFactory() =>
            Container
                .BindFactory<ProfitOfferModel, ProfitOfferView, ProfitOfferPresenter, ProfitOfferPresenter.Factory>()
                .AsSingle()
                .NonLazy();
        
        private void BindAdsGemsPackPresenterFactory() =>
            Container
                .BindFactory<AdsGemsPack, AdsGemsPackView, AdsGemsPackPresenter, AdsGemsPackPresenter.Factory>()
                .AsSingle()
                .NonLazy();
        
        private void BindFreeGemsPackPresenterFactory() =>
            Container
                .BindFactory<FreeGemsPack, FreeGemsPackView, FreeGemsPackPresenter, FreeGemsPackPresenter.Factory>()
                .AsSingle()
                .NonLazy();

        private void BindShopPresenter() =>
            Container
                .BindInterfacesAndSelfTo<ShopPresenter>()
                .AsSingle();

        private void BindMiniShopPresenter() =>
            Container
                .BindInterfacesAndSelfTo<MiniShopPresenter>()
                .AsSingle();
        
    }
}