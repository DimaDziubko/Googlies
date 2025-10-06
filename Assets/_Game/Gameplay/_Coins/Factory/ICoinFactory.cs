using _Game.Core.UserState._State;
using _Game.Gameplay._Coins.Scripts;
using _Game.Gameplay.Vfx.Scripts;

namespace _Game.Gameplay._Coins.Factory
{
    public interface ICoinFactory
    {
        public void Warmup();
        LootFlyingReward GetLootCoin();
        void Reclaim(FlyingReward flyingReward);
        FlyingCurrencyNew GetCurrencyVfx(CurrencyType type, CurrencyVfxRenderMode mode);
    }
}