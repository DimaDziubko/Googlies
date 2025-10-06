using _Game._BattleModes.Scripts;
using _Game.Core._DataPresenters._UpgradeItem;
using _Game.Core._GameSaver;
using _Game.Core._RetentionChecker;
using _Game.Core._Time;
using _Game.Core.Loading;
using _Game.Core.Navigation.Age;
using _Game.Core.Navigation.Battle;
using _Game.Core.Navigation.Timeline;
using _Game.Core.Notifications;
using _Game.Core.Pause.Scripts;
using _Game.Core.Services._BattleSpeedService._Scripts;
using _Game.Core.Services._SpeedBoostService.Scripts;
using _Game.Core.Services.IAP;
using _Game.Core.Services.Upgrades;
using _Game.Core.UserState._State;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._BattleSpeed.Scripts;
using _Game.Gameplay._Boosts.Scripts;
using _Game.Gameplay._Buyer;
using _Game.Gameplay._Cards.Scripts;
using _Game.Gameplay._GameEventRouter;
using _Game.Gameplay._ItemProvider;
using _Game.Gameplay._RewardItemResolver;
using _Game.Gameplay._RewardProcessing;
using _Game.Gameplay.BattleLauncher;
using _Game.LiveopsCore;
using _Game.LiveopsCore._GameEventCurrencyManagement;
using _Game.LiveopsCore._ShowcaseSystem;
using _Game.UI._CardsGeneral._Cards.Scripts;
using _Game.UI._Dungeons.Scripts;
using _Game.UI._TimelineInfoPresenter;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class GameplayServicesInstaller : MonoInstaller
    {
       
        public override void InstallBindings()
        {
            BindProductBuyer();
            
            BindUpgradeItemContainer();
            BindPauseManager();
            BindBattleSpeedManager();
            BindBeginGameManager();
            BindUpgradesCalculator();
            
            BindCardsBoostCalculator();
            BindProfitOfferBoostCalculator();
            
            BindUpgradesServices();
            BindNotificationService();
            BindSpeedBoostService();
            BindBattleSpeedService();
            
            BindEnvironmentFactoryMediator();
            BindBaseFactoryMediator();
            BindLoadingOperationFactory();

            BindTimelineLoader();
            BindAgeLoader();

            BindTimelineNavigator();
            BindAgeNavigator();
            BindBattleNavigator();

            BindRetentionChecker();
            BindDungeonManager();

            BindDungeonStrategyFactory();
            
            BindRewardProcessingService();
            BindRewardItemResolver();
            BindGameEventRouter();
            BindItemProvider();
            BindCardContainer();
            BindCardCollector();
            BindCardShower();
            
            BindTimeProvider();
            BindTargetRegistry();
            
            BindEventFactory();
            BindEventScheduler();
            BindEventStrategyFactory();
            BindEventSystem();
            BindEventListPresenter();
            BindGameEventsShowcaseSystem();
            BindGameEventContainer();
            BindGameEventPresenterFactory();
            BindGameEventTracker();

            BindPurchaseChecker();
            BindSaveMediator();

            BindEventPendingRewardProcessor();
            BindEventPendingRewardShower();
            
            BindLootDropChanceCalculator();
            
            BindBankSynchronizer();
        }

        private void BindGameEventTracker() =>
            Container.BindInterfacesAndSelfTo<GameEventTracker>()
                .AsSingle()
                .NonLazy();

        private void BindBankSynchronizer() =>
            Container.BindInterfacesAndSelfTo<BankSynchronizer>()
                .AsSingle()
                .NonLazy();

        private void BindLootDropChanceCalculator() =>
            Container.Bind<IUnitDropChanceCalculator>().To<BasicDropChanceCalculator>()
                .AsSingle()
                .NonLazy();

        private void BindEventPendingRewardShower() =>
            Container.BindInterfacesAndSelfTo<GameEventPendingRewardShower>()
                .AsSingle()
                .NonLazy();
        
        private void BindEventPendingRewardProcessor() =>
            Container.BindInterfacesAndSelfTo<GameEventPendingRewardProcessor>()
                .AsSingle()
                .NonLazy();
        
        private void BindSaveMediator() =>
            Container.BindInterfacesAndSelfTo<SaveGameMediator>()
                .AsSingle()
                .NonLazy();

        private void BindPurchaseChecker() =>
            Container.Bind<PurchaseChecker>()
                .AsSingle()
                .NonLazy();

        private void BindGameEventPresenterFactory() =>
            Container.BindInterfacesAndSelfTo<GameEventPresenterFactory>()
                .AsSingle()
                .NonLazy();
        
        private void BindGameEventContainer() =>
            Container.BindInterfacesAndSelfTo<GameEventContainer>()
                .AsSingle()
                .NonLazy();
        
        private void BindGameEventsShowcaseSystem() =>
            Container.BindInterfacesAndSelfTo<GameEventShowcaseSystem>()
                .AsSingle()
                .NonLazy();
        
        private void BindEventListPresenter() =>
            Container.BindInterfacesAndSelfTo<GameEventListPresenter>()
                .AsSingle()
                .NonLazy();
        
        private void BindEventSystem() =>
            Container.BindInterfacesAndSelfTo<GameEventSystem>()
                .AsSingle()
                .NonLazy();
        
        private void BindEventStrategyFactory() =>
            Container.BindInterfacesAndSelfTo<GameEventStrategyFactory>()
                .AsSingle()
                .NonLazy();
        
        private void BindEventScheduler() =>
            Container.BindInterfacesAndSelfTo<GameEventScheduler>()
                .AsSingle()
                .NonLazy();
        
        private void BindEventFactory() =>
            Container.BindInterfacesAndSelfTo<GameEventFactorySelector>()
                .AsSingle()
                .NonLazy();

        
        private void BindTargetRegistry()
        {
            Container.BindInterfacesAndSelfTo<TargetRegistry>()
                .AsSingle()
                .NonLazy();
        }

        private void BindCardShower() =>
            Container.BindInterfacesAndSelfTo<CardShower>()
                .AsSingle()
                .NonLazy();

        private void BindCardCollector() =>
            Container.BindInterfacesAndSelfTo<CardCollector>()
                .AsSingle()
                .NonLazy();
        
        private void BindCardContainer() =>
            Container.BindInterfacesAndSelfTo<CardContainer>()
                .AsSingle()
                .NonLazy();
        
        private void BindGameEventRouter()
        {
            Container.BindInterfacesAndSelfTo<GameEventRouter>()
                .AsSingle()
                .NonLazy();
        }
        
        private void BindRewardItemResolver() =>
            Container.BindInterfacesAndSelfTo<RewardItemResolver>()
                .AsSingle()
                .NonLazy();


        private void BindItemProvider() =>
            Container.BindInterfacesAndSelfTo<ItemProvider>()
                .AsSingle()
                .NonLazy();

        private void BindRewardProcessingService() =>
            Container.BindInterfacesAndSelfTo<RewardProcessingService>()
                .AsSingle()
                .NonLazy();

        private void BindProductBuyer()
        {
            Container.BindInterfacesAndSelfTo<ProductBuyer>()
                .AsSingle()
                .NonLazy();
        }
        
        private void BindTimeProvider() => 
            Container
            .BindInterfacesAndSelfTo<TimeProvider>()
            .AsSingle();

        private void BindBaseFactoryMediator() => 
            Container.Bind<BaseFactoryMediator>().AsSingle();

        private void BindEnvironmentFactoryMediator() => 
            Container.Bind<EnvironmentFactoryMediator>().AsSingle();
        
        private void BindTimelineLoader() =>
            Container.Bind<ITimelineLoader>()
                .To<TimelineLoader>()
                .AsSingle();

        private void BindAgeLoader() =>
            Container.Bind<IAgeLoader>()
                .To<AgeLoader>()
                .AsSingle();

        private void BindLoadingOperationFactory() => 
            Container.Bind<LoadingOperationFactory>().AsSingle();

        private void BindProfitOfferBoostCalculator() => 
            Container.BindInterfacesAndSelfTo<ProfitOfferBoostCalculator>().AsSingle();

        private void BindUpgradeItemContainer() => 
            Container.Bind<UpgradeItemContainer>().AsSingle();

        private void BindDungeonStrategyFactory() => 
            Container.Bind<DungeonStrategyFactory>().AsSingle();

        private void BindDungeonManager() => 
            Container.BindInterfacesAndSelfTo<DungeonsManager>().AsSingle();

        private void BindCardsBoostCalculator() =>
            Container.BindInterfacesAndSelfTo<CardsBoostCalculator>().AsSingle();

        private void BindRetentionChecker() =>
            Container.BindInterfacesAndSelfTo<RetentionChecker>().AsSingle();
        
        private void BindPauseManager() =>
            Container.Bind<IPauseManager>()
                .To<PauseManager>()
                .AsSingle();

        private void BindBattleSpeedManager() =>
            Container
                .BindInterfacesAndSelfTo<BattleSpeedManager>()
                .AsSingle();

        private void BindBeginGameManager() =>
            Container
                .BindInterfacesAndSelfTo<GameManager>()
                .AsSingle();

        private void BindUpgradesCalculator() =>
            Container
                .BindInterfacesAndSelfTo<UpgradeCalculator>()
                .AsSingle();

        private void BindUpgradesServices()
        {
            Container
                .BindInterfacesAndSelfTo<TimelineInfoPresenter>()
                .AsSingle();
        }
        
        private void BindNotificationService() =>
            Container.Bind<NotificationService>()
                .AsSingle();
        
        private void BindSpeedBoostService() =>
            Container
                .BindInterfacesAndSelfTo<SpeedBoostService>()
                .AsSingle();

        private void BindBattleSpeedService() =>
            Container
                .BindInterfacesAndSelfTo<GameSpeedService>()
                .AsSingle();

        private void BindBattleNavigator() =>
            Container
                .BindInterfacesAndSelfTo<BattleNavigator>()
                .AsSingle();

        private void BindAgeNavigator() =>
            Container
                .BindInterfacesAndSelfTo<AgeNavigator>()
                .AsSingle();

        private void BindTimelineNavigator() =>
            Container
                .BindInterfacesAndSelfTo<TimelineNavigator>()
                .AsSingle();
    }
}