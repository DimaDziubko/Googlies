using _Game._BattleModes.Scripts;
using _Game.Core._DataProviders.UnitDataProvider;
using _Game.Gameplay._Battle.Scripts;
using _Game.Gameplay._BattleField.Scripts;
using _Game.Gameplay._Skills;
using _Game.Gameplay._UnitBuilder.Scripts;
using _Game.Gameplay.Food.Scripts;
using _Game.LiveopsCore._GameEventCurrencyManagement;
using _Game.UI._Skills.Scripts;
using UnityEngine;
using Zenject;

namespace _Game.Core.Installers.Local.ZombieRushMode
{
    public class ZombieRushSystemInstaller : MonoInstaller
    {
        [SerializeField] private BattleFieldSettings _settings;
        public override void InstallBindings()
        {
            BindFoodGenerator();
            BindBaseDestructionManager();
            BindBattleField();
            BindUnitBuilderViewController();
            
            BindAmbienceController();
            BindZombieRushBattle();
            BindBattleMode();
            
            BinsSkillListPresenter();
            BindSkillStrategyFactory();

            BindGameEventEarningStrategyFactory();
        }

        private void BindGameEventEarningStrategyFactory() => 
            Container.BindInterfacesAndSelfTo<ZombieRushModeGameEventCurrencyEarnStrategyFactory>().AsSingle().NonLazy();

        private void BinsSkillListPresenter() => 
            Container.BindInterfacesAndSelfTo<SkillListPresenter>().AsSingle().NonLazy();
        private void BindSkillStrategyFactory()
        {
            var unitDataProvider = Container.Resolve<IZombieRushModeUnitDataProvider>();
    
            Container
                .Bind<SkillStrategyFactory>()
                .ToSelf()
                .AsSingle()
                .WithArguments(unitDataProvider)
                .NonLazy();
        }
        
        private void BindAmbienceController() => 
            Container.Bind<AmbienceController>().AsSingle();

        private void BindFoodGenerator() =>
            Container.BindInterfacesAndSelfTo<FoodGenerator>().AsSingle();
        
        private void BindBaseDestructionManager() => 
            Container.BindInterfacesAndSelfTo<BattleTriggersManager>().AsSingle();
        
        private void BindBattleField() =>
            Container.BindInterfacesAndSelfTo<ZombieRushBattleField>().AsSingle().WithArguments(_settings);
        
        private void BindUnitBuilderViewController() =>
            Container.BindInterfacesAndSelfTo<UnitBuilderViewController>().AsSingle();
        
        private void BindZombieRushBattle() =>
            Container.BindInterfacesAndSelfTo<ZombieRushBattle>().AsSingle().NonLazy();
        
        private void BindBattleMode() =>
            Container.BindInterfacesAndSelfTo<_BattleModes.Scripts.ZombieRushMode>().AsSingle().NonLazy();
        
    }
}