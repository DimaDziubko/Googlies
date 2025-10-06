using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Core.Navigation.Age;
using _Game.Gameplay._Battle.Scripts;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._CoinCounter.Scripts;
using _Game.Gameplay._DailyTasks.Scripts;
using _Game.Gameplay._Skills;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay.Food.Scripts;
using _Game.LiveopsCore._GameEventCurrencyManagement;
using _Game.UI._BattleResultPopup.Scripts;
using _Game.UI._Hud._DailyTaskView;
using _Game.UI._Skills.Scripts;
using UnityEngine;
using Zenject;

namespace _Game.Core.Installers.Local.BattleMode
{
    public class BattleSystemInstaller : MonoInstaller
    {
        [SerializeField] private BattleFieldSettings _battleFieldSettings;
        public override void InstallBindings()
        {
            BindFoodGenerator();
            BindCoinCounter();
            BindBaseDestructionManager();
            
            BindBattleField();
            BindUnitBuilderViewController();
            
            BindAmbienceController();
            BindSoundController();
            BindBattle();
            
            BindBattleResultPopupController();
            BindBattleMode();

            BindProgressResetHandler();
            
            BindStartBattleScreenEventsRouter();
            BindDailyTaskPresenterFactory();
            BindDailyTaskGenerator();
            BindDailyTaskFactory();
            BindDailyTaskScheduler();

            BindBattleStatisticsController();
            
            BinsSkillListPresenter();

            BindSkillStrategyFactory();
            BindSkillPotionController();

            BindGameEventEarningStrategyFactory();
        }

        private void BindGameEventEarningStrategyFactory() => 
            Container
                .Bind<IGameEventCurrencyEarnStrategyFactory>()
                .To<BattleModeGameEventCurrencyEarnStrategyFactory>()
                .AsSingle()
                .NonLazy();
        
        private void BindSkillPotionController() =>
            Container.BindInterfacesAndSelfTo<SkillPotionController>().AsSingle().NonLazy();
        private void BindSkillStrategyFactory()
        {
            var unitDataProvider = Container.Resolve<IBattleModeUnitDataProvider>();
    
            Container
                .Bind<SkillStrategyFactory>()
                .ToSelf()
                .AsSingle()
                .WithArguments(unitDataProvider)
                .NonLazy();
        }
        private void BinsSkillListPresenter() =>
            Container.BindInterfacesAndSelfTo<SkillListPresenter>().AsSingle().NonLazy();
        private void BindStartBattleScreenEventsRouter() =>
            Container.BindInterfacesAndSelfTo<StartBattleScreenEventsRouter>().AsSingle();

        private void BindBattleStatisticsController() => 
            Container.BindInterfacesAndSelfTo<BattleStatisticsController>().AsSingle();

        private void BindDailyTaskPresenterFactory() =>
            Container
                .BindFactory<DailyTaskModel, DailyTaskView,  DailyTaskPresenter, DailyTaskPresenter.Factory>()
                .AsSingle()
                .NonLazy();
        
        private void BindDailyTaskScheduler() => 
            Container.BindInterfacesTo<DailyTaskScheduler>().AsSingle().NonLazy();

        private void BindDailyTaskFactory() => 
            Container.Bind<DailyTaskStrategyFactory>().AsSingle();

        private void BindDailyTaskGenerator() => 
            Container.Bind<DailyTaskGenerator>().AsSingle();

        private void BindProgressResetHandler() => 
            Container.BindInterfacesTo<AgeProgressResetHandler>().AsSingle();

        private void BindBattleResultPopupController() => 
            Container.Bind<BattleResultPopupController>().AsSingle();

        private void BindSoundController() => 
            Container.Bind<AmbienceController>().AsSingle();

        private void BindAmbienceController() => 
            Container.Bind<GameSoundController>().AsSingle();

        private void BindFoodGenerator() =>
            Container.BindInterfacesAndSelfTo<FoodGenerator>().AsSingle();

        private void BindCoinCounter() =>
            Container.BindInterfacesAndSelfTo<CoinCounter>().AsSingle();

        private void BindBaseDestructionManager() => 
            Container.BindInterfacesAndSelfTo<BattleTriggersManager>().AsSingle();
        
        private void BindBattleField() =>
            Container.BindInterfacesAndSelfTo<BattleField>().AsSingle().WithArguments(_battleFieldSettings);

        private void BindUnitBuilderViewController() =>
            Container.BindInterfacesAndSelfTo<UnitBuilderViewController>().AsSingle();

        private void BindBattle() =>
            Container.BindInterfacesAndSelfTo<Battle>().AsSingle().NonLazy();
        
        private void BindBattleMode() =>
            Container.BindInterfacesAndSelfTo<_BattleModes.Scripts.BattleMode>().AsSingle().NonLazy();
        
    }
}