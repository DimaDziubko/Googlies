using _Game.Core.UserState._State;

namespace _Game.Gameplay._RewardProcessing
{
    public class MoneyItemLocal : ItemLocal
    {
        public override int Id { get; set; }
        public override string IconKey { get; set; }
        public CurrencyType CurrencyType;
        
        public static MoneyItemLocal Default()
        {
            return new MoneyItemLocal()
            {
                Id = 829,
                IconKey = "Gem_0",
                CurrencyType = CurrencyType.Gems,
            };
        }
    }
}