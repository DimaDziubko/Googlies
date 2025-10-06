using _Game.Core._DataPresenters._WeaponDataProvider;
using _Game.Core._DataProviders._BaseDataProvider;
using _Game.Core._DataProviders.UnitDataProvider;
using Zenject;

namespace _Game.Core.Installers.Core
{
    public class DataProvidersInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindDungeonModelFactory();
            
            BindBattleModeUnitDataProvider();
            BindZombieRushModeUnitDataProvider();
            
            BindBattleModeWeaponDataProvider();
            BindZombieRushModeWeaponDataProvider();
            
            BindBattleModeBaseDataProvider();
            BindZombieRushModeBaseDataProvider();
        }

        private void BindZombieRushModeBaseDataProvider() => 
            Container.BindInterfacesAndSelfTo<BattleModeBaseDataProvider>().AsSingle().NonLazy();

        private void BindBattleModeBaseDataProvider() => 
            Container.BindInterfacesAndSelfTo<ZombieRushModeBaseDataProvider>().AsSingle().NonLazy();

        private void BindZombieRushModeUnitDataProvider() => 
            Container.BindInterfacesAndSelfTo<ZombieRushModeUnitDataProvider>().AsSingle().NonLazy();

        private void BindBattleModeUnitDataProvider() => 
            Container.BindInterfacesAndSelfTo<BattleModeUnitDataProvider>().AsSingle().NonLazy();

        private void BindZombieRushModeWeaponDataProvider() => 
            Container.BindInterfacesAndSelfTo<ZombieRushModeWeaponDataProvider>().AsSingle().NonLazy();

        private void BindBattleModeWeaponDataProvider() => 
            Container.BindInterfacesAndSelfTo<BattleModeWeaponDataProvider>().AsSingle().NonLazy();

        private void BindDungeonModelFactory() => 
            Container.BindInterfacesAndSelfTo<DungeonModelFactory>().AsSingle();
        
    }
}