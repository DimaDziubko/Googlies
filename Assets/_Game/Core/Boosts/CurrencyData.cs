using _Game.Core.UserState._State;

namespace _Game.Core.Boosts
{
    public struct CurrencyData
    {
        public CurrencyType Type;
        public float Amount;
        public ItemSource Source;
    }

    public enum ItemSource
    {
        None,
        Shop,
        MiniShop,
        FreeGemsPack,
        AdsGemsPack,
        Upgrade,
        DailyTask,
        Cards,
        CoinsBundle,
        Dungeons,
        Skills,
        FortuneSlotReward,
        Evolve,
        CardsScreen,
        Ad,
        LeaderPass,
        ClassicOffer
    }
}