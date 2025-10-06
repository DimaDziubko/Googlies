using System;
using _Game.Core._FeatureUnlockSystem.Scripts;
using _Game.Core._GameListenerComposite;
using _Game.Core._Logger;
using _Game.Core.Configs.Repositories;
using _Game.Core.Configs.Repositories._IconConfigRepository;
using _Game.Core.Configs.Repositories.Economy;
using _Game.Core.Services.UserContainer;
using _Game.Core.UserState._State;
using _Game.Gameplay.Food.Scripts;
using _Game.UI._Hud._FoodBoostView;
using UnityEngine;
using Zenject;

namespace _Game.Gameplay._FoodBoost.Scripts
{
    public class FoodBoostService :
        IInitializable, 
        IDisposable, 
        IStartGameListener
    {
        private readonly IFoodContainer _foodContainer;
        
        private readonly IUserContainer _userContainer;
        private readonly IFeatureUnlockSystem _featureUnlockSystem;
        private readonly IMyLogger _logger;

        private readonly IConfigRepository _config;
        private IIconConfigRepository IconConfig => _config.IconConfigRepository;
        private IEconomyConfigRepository Economy => _config.EconomyConfigRepository;
        private IFoodBoostStateReadonly FoodBoostState => _userContainer.State.FoodBoost;
        public FoodBoostBtnModel Model => _foodBoostBtnModel;

        private FoodBoostBtnModel _foodBoostBtnModel;

        public FoodBoostService(
            IUserContainer userContainer,
            IConfigRepository config,
            IMyLogger logger)
        {
            _userContainer = userContainer;
            _config = config;
            _logger = logger;
        }

        void IInitializable.Initialize()
        {
            CheckRecovery();
            
            _foodBoostBtnModel = 
                new FoodBoostBtnModel(
                    _userContainer.State.FoodBoost, 
                    Economy.GetFoodBoostConfig(), 
                    IconConfig.FoodIcon());
        }
        

        private void CheckRecovery()
        {
            var foodBoostConfig = Economy.GetFoodBoostConfig();

            DateTime now = DateTime.UtcNow;
            
            if (now.Date > FoodBoostState.LastDailyFoodBoost.Date)
            {
                _userContainer.FoodBoostStateHandler.RecoverFoodBoost(
                    foodBoostConfig.DailyFoodBoostCount, 
                    now.Date);
                return;
            }

            TimeSpan timeSinceLastBoost = now - FoodBoostState.LastDailyFoodBoost;
            int recoverableBoosts = (int)(timeSinceLastBoost.TotalMinutes / foodBoostConfig.RecoverTimeMinutes);

            if (recoverableBoosts > 0)
            {
                int lackingBoosts = foodBoostConfig.DailyFoodBoostCount - FoodBoostState.DailyFoodBoostCount;
                int boostsToAdd = Mathf.Min(recoverableBoosts, lackingBoosts);

                DateTime newLastDailyFoodBoost = FoodBoostState.LastDailyFoodBoost.AddMinutes(boostsToAdd * foodBoostConfig.RecoverTimeMinutes);
                _userContainer.FoodBoostStateHandler.RecoverFoodBoost(
                    boostsToAdd,
                    newLastDailyFoodBoost);
            }
        }
        
        void IStartGameListener.OnStartBattle() => CheckRecovery();

        void IDisposable.Dispose()
        {
            
        }
    }
}