using _Game._BattleModes.Scripts;
using _Game.Core._DataPresenters.UnitBuilderDataPresenter;
using _Game.Core._DataProviders._FoodDataProvider;
using _Game.Core.Services._Camera;
using _Game.Gameplay._CoinCounter.Scripts;
using _Game.Gameplay._FoodBoost.Scripts;
using _Game.Gameplay.Food.Scripts;
using _Game.UI._BattleHudController;
using _Game.UI._BattleResultPopup.Scripts;
using _Game.UI._Currencies;
using _Game.UI._Environment;
using _Game.UI._GameplayUI.Scripts;
using _Game.UI._Header.Scripts;
using _Game.UI._Hud;
using _Game.UI._Hud._FoodBoostView;
using _Game.UI._Hud._PauseView;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace _Game.Core.Installers.Local.BattleMode
{
    public class BattleUIInstaller : MonoInstaller
    {
        [SerializeField, Required] private Header _header;
        [SerializeField, Required] private BattleHud battleHud;
        [SerializeField, Required] private GameplayUI gameplayUI;

        public override void InstallBindings()
        {
            BindFoodProductionDataProvider();
            BindFoodContainer();
            BindFoodBoostService();
            
            BindHud();
            BindBattleUnitBuilderModel();
            BindGameplayUI();
            BindCurrencyAnimation();
            BindCurrencyAnimationController();
            BindBattleUIController();
            
            BindEnvironmentController();

            BindFoodPanelPresenter();

            BindCoinsAndGemsPresenter();
            BindHeaderPresenter();

            BindPausePresenter();
            BindCoinCounterPresenter();
            BindFoodBoostPresenter();


            BindBattleResultPopupPresenter();
            BindBattleResultPopupProvider();

            //BindWalletSynchronizer();
        }

        private void BindWalletSynchronizer() => 
            Container.BindInterfacesAndSelfTo<BankSynchronizer>().AsSingle().NonLazy();

        private void BindBattleResultPopupProvider() => 
            Container.Bind<IBattleResultPopupProvider>().To<BattleResultPopupProvider>().AsSingle().NonLazy();

        private void BindBattleResultPopupPresenter() => 
            Container.BindInterfacesAndSelfTo<BattleResultPopupPresenter>().AsSingle().NonLazy();

        private void BindFoodBoostPresenter() => 
            Container.BindInterfacesTo<FoodBoostPresenter>().AsSingle().NonLazy();

        private void BindCoinCounterPresenter() => 
            Container.BindInterfacesTo<CoinCounterPresenter>().AsSingle().NonLazy();

        private void BindPausePresenter() => 
            Container.BindInterfacesTo<BattlePausePresenter>().AsSingle().NonLazy();

        private void BindCurrencyAnimation() =>
            Container.BindInterfacesAndSelfTo<CurrencyAnimation>().AsSingle().NonLazy();
        
        private void BindCurrencyAnimationController() => 
            Container.BindInterfacesAndSelfTo<CurrencyAnimationController>().AsSingle().NonLazy();

        private void BindHeaderPresenter() => 
            Container.BindInterfacesAndSelfTo<HeaderPresenter>().AsSingle().WithArguments(_header).NonLazy();

        private void BindCoinsAndGemsPresenter() => 
            Container.BindInterfacesAndSelfTo<CoinsAndGemsPresenter>().AsSingle().WithArguments(_header).NonLazy();

        private void BindFoodProductionDataProvider() => 
            Container.BindInterfacesAndSelfTo<BattleFoodProductionProvider>().AsSingle().NonLazy();

        private void BindFoodBoostService() => 
            Container.BindInterfacesAndSelfTo<FoodBoostService>().AsSingle().NonLazy();

        private void BindFoodContainer() =>
            Container.Bind<IFoodContainer>().To<FoodContainer>().AsSingle().NonLazy();

        private void BindFoodPanelPresenter() => 
            Container.BindInterfacesAndSelfTo<FoodPanelPresenter>().AsSingle().NonLazy();

        private void BindBattleUnitBuilderModel() => 
            Container.BindInterfacesAndSelfTo<BattleUnitBuilderModel>().AsSingle().NonLazy();

        private void BindHud() => 
            Container.Bind<BattleHud>().FromInstance(battleHud).AsSingle().NonLazy();

        private void BindBattleUIController() => 
            Container.BindInterfacesAndSelfTo<GameHudController>().AsSingle().NonLazy();
        
        private void BindGameplayUI()
        {
            var cameraService = Container.Resolve<IWorldCameraService>();
            gameplayUI.Construct(cameraService);
            Container.BindInterfacesAndSelfTo<GameplayUI>().FromInstance(gameplayUI).AsSingle();
        }

        private void BindEnvironmentController() => 
            Container.Bind<BattleEnvironmentController>().AsSingle().NonLazy();
    }
}