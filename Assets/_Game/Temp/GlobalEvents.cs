using System;

namespace _Game.Temp
{
    public static class GlobalEvents
    {
        public static event Action OnInsufficientFunds;

        public static void RaiseOnInsufficientFunds()
        {
            OnInsufficientFunds?.Invoke();
        }
    }
}