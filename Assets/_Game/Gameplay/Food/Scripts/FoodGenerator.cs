using System;
using _Game.Core._DataProviders._FoodDataProvider;
using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.CustomKernel;

namespace _Game.Gameplay.Food.Scripts
{
    public class FoodGenerator :
        IGameTickable,
        IStartGameListener,
        IStopGameListener,
        IGameSpeedListener,
        IPauseListener
    {
        public event Action<int> FoodProduced;

        private readonly IFoodProductionProvider _provider;
        private readonly IMyLogger _logger;

        private float _defaultProductionSpeed;
        private float _productionSpeed;

        private int _foodAmount;
        private float _accumulatedProgress;

        private readonly FoodPanel _panel;

        private readonly IFoodContainer _foodContainer;
        
        private bool _isPaused;
        
        public FoodGenerator(
            IMyLogger logger,
            IFoodContainer foodContainer)
        {
            _foodContainer = foodContainer;
            _logger = logger;
        }

        void IStartGameListener.OnStartBattle() => StartGenerator();

        private void StartGenerator()
        {
            _foodContainer.Reset();
            _defaultProductionSpeed = _foodContainer.ProductionSpeed;
        }
        
        void IGameTickable.Tick(float deltaTime)
        {
            if(_isPaused) return;
            
            _accumulatedProgress += deltaTime * _productionSpeed;
            
            if (_accumulatedProgress > 1f)
            {
                var producedFood = (int)_accumulatedProgress;
                _foodContainer.Add(producedFood);
                FoodProduced?.Invoke(producedFood);
                _foodContainer.SetProgress(1f);
                _accumulatedProgress %= 1f;
            }
            else
            {
                _foodContainer.SetProgress(_accumulatedProgress);
            }
        }
        
        void IStopGameListener.OnStopBattle()
        {
            _foodContainer.Clear();
            _foodContainer.SetProgress(0f);
            _accumulatedProgress = 0;
        }

        void IGameSpeedListener.OnBattleSpeedFactorChanged(float speedFactor)
        {
            _productionSpeed = _defaultProductionSpeed * speedFactor;
        }

        public void SetPaused(bool isPaused)
        {
            _isPaused = isPaused;
        }
    }
}