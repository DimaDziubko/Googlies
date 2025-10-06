using System;

namespace _Game.Core.UserState._Handler._FoodBoost
{
    public interface IFoodBoostStateHandler
    {
        void RecoverFoodBoost(int amount, DateTime lastDailyFoodBoost);
        void SpendFoodBoost(DateTime lastDailyFoodBoost);
    }
}