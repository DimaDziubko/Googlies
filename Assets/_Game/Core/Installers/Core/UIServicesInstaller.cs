using _Game.Core.Boosts;
using _Game.Core.Data;
using _Game.Core.LoadingScreen;
using _Game.Core.Navigation.Timeline;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Gameplay._Tutorial.Scripts;
using _Game.UI._AlertPopup;
using _Game.UI._BattleResultPopup.Scripts;
using _Game.UI._BoostPopup;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.UI._CardsGeneral.Scripts;
using _Game.UI._Dungeons.Scripts;
using _Game.UI._EvolveScreen.Scripts;
using _Game.UI._Hud;
using _Game.UI._MainMenu.Scripts;
using _Game.UI._ParticleAttractorSystem;
using _Game.UI._Shop._MiniShop.Scripts;
using _Game.UI._Shop.Scripts._ShopScr;
using _Game.UI._Skills.Scripts;
using _Game.UI._StartBattleScreen.Scripts;
using _Game.UI._StatsPopup._Scripts;
using _Game.UI._TimelineInfoScreen.Scripts;
using _Game.UI._TravelScreen.Scripts;
using _Game.UI._UpgradesAndEvolution.Scripts;
using _Game.UI._UpgradesScreen.Scripts;
using _Game.UI.Factory;
using _Game.UI.Global;
using _Game.UI.RateGame.Scripts;
using _Game.UI.Settings.Scripts;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class UIServicesInstaller : MonoInstaller
    {
        [SerializeField, Required] private TutorialPointersParent _tutorialPointerParent;
        [SerializeField, Required] private UIFactory _uiFactory;
        [SerializeField, Required] private Curtain _curtain;
        [SerializeField, Required] private CardAppearancePopupSettings _cardAppearancePopupSettings;

        public override void InstallBindings()
        {
            BindMainMenuStateFactory();
            BindMainMenu();
            BindMainMenuProvider();

            BindBoostsContainer();

            BindBoostPopupPresenter();
            BindBoostPopupProvider();

            BindQuickBoostInfoPresenterFactory();
            BindBoostDataPresenterFactory();

            BindUINotifier();

            BindCurtain();
            BindAlertPopupProvider();
            BindTutorialPointersParent();
            BindUIFactory();
            BindTutorialManager();
            BindShopProvider();
            BindMiniShopProvider();
            BindLoadingScreenProvider();
            BindISettingsPopupProvider();

            BindStartBattleScreenPresenter();
            BindStartBattleScreenProvider();

            BindCardsScreenPresenter();
            BindUpgradesScreenPresenter();
            BindEvolveScreenPresenter();
            BindTravelScreenPresenter();

            BindTravelScreenProvider();
            BindEvolutionScreenProvider();

            BindCardsScreenProvider();

            BindGeneralCardsScreenPresenter();
            BindGeneralCardsScreenProvider();

            // BindUpgradesAndEvolutionScreenPresenter();
            BindUpgradesScreenProvider();

            BindGameResultPopupProvider();
            BindTimelineInfoScreenProvider();

            BindGameRateScreenProvider();
            BindRateGameChecker();

            BindStatsPopupPresenter();
            BindStatsPopupProvider();

            BindDungeonPresenterFactory();
            BindDungeonsScreenPresenter();
            BindDungeonsScreenProvider();

            BindSkillScreenProvider();
            BindSkillsScreenPresenter();
            BindSkillService();

            BindCardAppearanceScreenProvider();

            BindParticleAttractorRegistry();
        }

        private void BindParticleAttractorRegistry() =>
            Container.BindInterfacesAndSelfTo<ParticleAttractorRegistry>()
                .AsSingle();

        private void BindCardAppearanceScreenProvider() =>
            Container.Bind<ICardAppearanceScreenProvider>()
                .To<CardAppearanceScreenProvider>()
                .AsSingle()
                .WithArguments(_cardAppearancePopupSettings);

        private void BindSkillService() =>
            Container.BindInterfacesAndSelfTo<SkillService>()
                .AsSingle();

        private void BindSkillsScreenPresenter() =>
            Container
                .BindInterfacesAndSelfTo<SkillsScreenPresenter>()
                .AsSingle();

        private void BindSkillScreenProvider() =>
            Container.Bind<ISkillsScreenProvider>()
                .To<SkillsScreenProvider>()
                .AsSingle();

        private void BindStatsPopupPresenter() =>
            Container.Bind<IStatsPopupPresenter>()
                .To<StatsPopupPresenter>()
                .AsSingle();

        private void BindBoostPopupPresenter() =>
            Container.Bind<IBoostPopupPresenter>()
                .To<BoostPopupPresenter>()
                .AsSingle();

        private void BindGeneralCardsScreenPresenter() =>
            Container.BindInterfacesTo<GeneralCardsScreenPresenter>().AsSingle();

        private void BindCardsScreenPresenter() =>
            Container.BindInterfacesTo<CardsScreenPresenter>().AsSingle();

        private void BindUpgradesScreenPresenter() =>
            Container.BindInterfacesTo<UpgradesScreenPresenter>().AsSingle();

        private void BindEvolveScreenPresenter() =>
            Container.BindInterfacesAndSelfTo<EvolveScreenPresenter>().AsSingle();

        private void BindTravelScreenPresenter() =>
            Container.BindInterfacesAndSelfTo<TravelScreenPresenter>().AsSingle();

        private void BindTravelScreenProvider() =>
            Container.BindInterfacesTo<TravelScreenProvider>().AsSingle();

        private void BindEvolutionScreenProvider() =>
            Container.BindInterfacesTo<EvolveScreenProvider>().AsSingle();

        private void BindUpgradesScreenProvider() =>
            Container.BindInterfacesTo<UpgradesScreenProvider>().AsSingle().NonLazy();

        private void BindUpgradesAndEvolutionScreenPresenter() =>
            Container
                .BindInterfacesAndSelfTo<UpgradesAndEvolutionScreenPresenter>()
                .AsSingle();

        private void BindStartBattleScreenPresenter() =>
            Container
                .BindInterfacesAndSelfTo<StartBattleScreenPresenter>()
                .AsSingle();

        private void BindMainMenu() =>
            Container.BindInterfacesAndSelfTo<MainMenu>().AsSingle();

        private void BindMainMenuStateFactory() =>
            Container.Bind<MenuStateFactory>().AsSingle();

        private void BindUINotifier() =>
            Container.BindInterfacesAndSelfTo<UINotifier>().AsSingle();

        private void BindBoostDataPresenterFactory()
        {
            Container
                .BindFactory<BoostModel, QuickBoostInfoItem, QuickBoostDataPresenter,
                    QuickBoostDataPresenter.Factory>()
                .AsSingle()
                .NonLazy();

            Container
                .BindFactory<BoostModel, BoostInfoItem, BoostDataPresenter,
                    BoostDataPresenter.Factory>()
                .AsSingle()
                .NonLazy();
        }

        private void BindQuickBoostInfoPresenterFactory()
        {
            Container
                .BindFactory<QuickBoostInfoPanel, QuickBoostInfoPresenter, QuickBoostInfoPresenter.Factory>()
                .AsSingle()
                .NonLazy();
        }

        private void BindBoostsContainer() =>
            Container.Bind<BoostContainer>().AsSingle();

        private void BindBoostPopupProvider() =>
            Container.BindInterfacesAndSelfTo<BoostPopupProvider>().AsSingle();

        private void BindDungeonPresenterFactory()
        {
            Container
                .BindFactory<IDungeonModel, DungeonView, DungeonPresenter, DungeonPresenter.Factory>()
                .AsSingle()
                .NonLazy();
        }

        private void BindDungeonsScreenProvider() =>
            Container.BindInterfacesTo<DungeonsScreenProvider>().AsSingle();

        private void BindDungeonsScreenPresenter() =>
            Container.BindInterfacesTo<DungeonsScreenPresenter>().AsSingle();

        private void BindCurtain() =>
            Container.Bind<Curtain>().FromInstance(_curtain).AsSingle();

        private void BindGameRateScreenProvider() =>
            Container.BindInterfacesTo<RateGameScreenProvider>().AsSingle();

        private void BindTutorialPointersParent()
        {
            Container
                .Bind<TutorialPointersParent>()
                .FromInstance(_tutorialPointerParent)
                .AsSingle();
        }

        private void BindTutorialManager() =>
            Container.BindInterfacesAndSelfTo<TutorialManager>().AsSingle();


        private void BindAlertPopupProvider() =>
            Container.Bind<IAlertPopupProvider>()
                .To<AlertPopupProvider>()
                .AsSingle();

        private void BindShopProvider() =>
            Container.Bind<IShopProvider>()
                .To<ShopProvider>()
                .AsSingle();

        private void BindMiniShopProvider()
        {
            Container.BindInterfacesTo<MiniShopProvider>()
                .AsSingle()
                .NonLazy();
        }

        private void BindLoadingScreenProvider() =>
            Container.Bind<ILoadingScreenProvider>()
                .To<LoadingScreenProvider>()
                .AsSingle();

        private void BindISettingsPopupProvider() =>
            Container
                .BindInterfacesAndSelfTo<SettingsPopupProvider>()
                .AsSingle();

        private void BindMainMenuProvider() =>
            Container
                .BindInterfacesAndSelfTo<MainMenuProvider>()
                .AsSingle();

        private void BindStartBattleScreenProvider() =>
            Container
                .BindInterfacesAndSelfTo<StartBattleScreenProvider>()
                .AsSingle();

        private void BindGameResultPopupProvider() =>
            Container
                .BindInterfacesTo<BattleResultPopupProvider>()
                .AsSingle();

        private void BindTimelineInfoScreenProvider() =>
            Container
                .BindInterfacesAndSelfTo<TimelineInfoScreenProvider>()
                .AsSingle();

        private void BindRateGameChecker() =>
            Container
                .BindInterfacesAndSelfTo<RateGameChecker>()
                .AsSingle();

        private void BindUIFactory()
        {
            _uiFactory.Initialize();
            Container.BindInterfacesAndSelfTo<UIFactory>().FromInstance(_uiFactory).AsSingle();
        }

        private void BindCardsScreenProvider() =>
            Container.Bind<ICardsScreenProvider>()
                .To<CardsScreenProvider>()
                .AsSingle();

        private void BindGeneralCardsScreenProvider() =>
            Container.Bind<IGeneralCardsScreenProvider>()
                .To<GeneralCardsScreenProvider>()
                .AsSingle();

        private void BindStatsPopupProvider() =>
            Container.Bind<IStatsPopupProvider>()
                .To<StatsPopupProvider>()
                .AsSingle();
    }
}