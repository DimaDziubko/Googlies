using System;
using _Game.Core.Configs.Models;
using _Game.Core.Configs.Models._FoodBoostConfig;
using _Game.Core.UserState._State;
using UnityEngine;

namespace _Game.UI._Hud._FoodBoostView
{
    public class FoodBoostBtnModel
    {
        private readonly FoodBoostState _state;
        private readonly FoodBoostConfig _config;
        private readonly Sprite _icon;
        
        public FoodBoostBtnModel(
            FoodBoostState state, 
            FoodBoostConfig config,
            Sprite icon)
        {
            _state = state;
            _config = config;
            _icon = icon;
        }
        
        public bool IsAvailableBoost => _state.DailyFoodBoostCount > 0;
        private int DailyFoodBoostCount => _config.DailyFoodBoostCount;
        public int Coefficient => _config.FoodBoostCoefficient;
        public Sprite Icon => _icon;

        public void SpendFoodBoost()
        {
            if (_state.DailyFoodBoostCount == DailyFoodBoostCount)
            {
                _state.ChangeFoodBoostCount(-1, DateTime.UtcNow);
            }
            else
            {
                _state.ChangeFoodBoostCount(-1, _state.LastDailyFoodBoost);
            }

        }
    }
}