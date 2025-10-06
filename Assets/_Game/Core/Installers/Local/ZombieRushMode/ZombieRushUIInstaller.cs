using _Game.Core._DataPresenters.UnitBuilderDataPresenter;
using _Game.Core._DataProviders._FoodDataProvider;
using _Game.Core.Services._Camera;
using _Game.Gameplay._Battle.Scripts;
using _Game.Gameplay.Food.Scripts;
using _Game.UI._Currencies;
using _Game.UI._Environment;
using _Game.UI._GameplayUI.Scripts;
using _Game.UI._Header.Scripts;
using _Game.UI._Hud;
using _Game.UI._Hud._PauseView;
using UnityEngine;
using Zenject;

namespace _Game.Core.Installers.Local.ZombieRushMode
{
    public class ZombieRushUIInstaller : MonoInstaller
    {
        [SerializeField] private Header  _header;
        [SerializeField] private ZombieRushHud _hud;
        [SerializeField] private GameplayUI gameplayUI;

        public override void InstallBindings()
        {
            BindFoodProductionProvider();
            BindFoodContainer();
            
            BindHud();
            BindZombieRushUnitBuilderModel();
            BindGameplayUI();
            BindDungeonEnvironmentController();

            BindFoodPanelPresenter();
            BindCoinsAndGemsPresenter();
            BindHeaderPresenter();

            BindResultPopupController();

            BindPausePresenter();
            BindHudController();
        }
        private void BindHeaderPresenter() => 
            Container.BindInterfacesAndSelfTo<HeaderPresenter>().AsSingle().WithArguments(_header).NonLazy();
        private void BindHudController() => 
            Container.Bind<ZombieRushHudController>().AsSingle();

        private void BindPausePresenter() => 
            Container.BindInterfacesTo<DungeonPausePresenter>().AsSingle();

        private void BindResultPopupController() => 
            Container.Bind<DungeonResultPopupController>().AsSingle();

        private void BindCoinsAndGemsPresenter() => 
            Container.BindInterfacesAndSelfTo<CoinsAndGemsPresenter>().AsSingle().WithArguments(_header).NonLazy();
        
        private void BindFoodPanelPresenter() => 
            Container.BindInterfacesAndSelfTo<FoodPanelPresenter>().AsSingle();
        
        private void BindZombieRushUnitBuilderModel() => 
            Container.BindInterfacesAndSelfTo<ZombieRushUnitBuilderModel>().AsSingle();

        private void BindFoodContainer() =>
            Container.Bind<IFoodContainer>().To<FoodContainer>().AsSingle();
        
        private void BindFoodProductionProvider() => 
            Container.BindInterfacesAndSelfTo<ZombieRushFoodProductionProvider>().AsSingle();
        
        private void BindDungeonEnvironmentController() => 
            Container.BindInterfacesAndSelfTo<DungeonEnvironmentController>().AsSingle();
        
        private void BindHud() =>
            Container.Bind<ZombieRushHud>().FromInstance(_hud).AsSingle();

        private void BindGameplayUI()
        {
            var cameraService = Container.Resolve<IWorldCameraService>();
            gameplayUI.Construct(cameraService);
            Container.BindInterfacesAndSelfTo<GameplayUI>().FromInstance(gameplayUI).AsSingle();
        }
    }
}