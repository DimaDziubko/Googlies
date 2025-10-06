using _Game.Core._GameListenerComposite;
using _Game.UI._GameplayUI.Scripts;

namespace _Game.Gameplay.Food.Scripts
{
    public class FoodPanelPresenter : IStartGameListener, IStopGameListener
    {
        private IFoodContainer _container;
        private GameplayUI _gameplayUI;

        private FoodPanel FoodPanel => _gameplayUI.FoodPanel;

        public FoodPanelPresenter(
            IFoodContainer container,
            GameplayUI gameplayUI)
        {
            _container = container;
            _gameplayUI = gameplayUI;
        }
        
        void IStartGameListener.OnStartBattle()
        {
            _container.OnProgressChanged += OnProgressChanged;
            _container.OnStateChanged += OnAmountChanged;
            FoodPanel.SetupIcon(_container.FoodIcon);
            OnProgressChanged(_container.Progress);
            OnAmountChanged();
        }

        void IStopGameListener.OnStopBattle()
        {
            _container.OnProgressChanged -= OnProgressChanged;
            _container.OnStateChanged -= OnAmountChanged;
        }

        private void OnAmountChanged() => FoodPanel.OnFoodChanged(_container.Amount);
        private void OnProgressChanged(float progress) => FoodPanel.UpdateFillAmount(progress, _container.ProductionSpeed);
    }
}