using _Game.Core.UserState._State;
using _Game.Gameplay._Coins.Factory;

namespace _Game.UI._Currencies
{
    public interface ICurrencyAnimation
    {
        void PlayCurrency(CurrencyType type, CurrencyVfxRenderMode renderMode);
    }
}