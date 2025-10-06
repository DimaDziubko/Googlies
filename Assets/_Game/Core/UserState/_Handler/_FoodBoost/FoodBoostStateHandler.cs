using System;
using _Game.Core.Services.UserContainer;

namespace _Game.Core.UserState._Handler._FoodBoost
{
    public class FoodBoostStateHandler : IFoodBoostStateHandler
    {
        private readonly IUserContainer _userContainer;

        public FoodBoostStateHandler(IUserContainer userContainer)
        {
            _userContainer = userContainer;
        }

        public void RecoverFoodBoost(int amount, DateTime lastDailyFoodBoost)
        {
            ChangeFoodBoost(amount, true, lastDailyFoodBoost);
            _userContainer.RequestSaveGame();

        }

        public void SpendFoodBoost(DateTime lastDailyFoodBoost)
        {
            ChangeFoodBoost(1, false, lastDailyFoodBoost);
            _userContainer.RequestSaveGame();

        }

        private void ChangeFoodBoost(int delta, bool isPositive, DateTime lastDailyFoodBoost)
        {
            delta = isPositive ? delta : (delta * -1);
            _userContainer.State.FoodBoost.ChangeFoodBoostCount(delta, lastDailyFoodBoost);
            _userContainer.RequestSaveGame();
        }
    }
}