namespace _Game.Core._Reward
{
    public class RewardItem
    {
        public int Id;
        public int Amount;
        public bool IsRewardClaimed;

        public void SetClaimed(bool isClaimed) => 
            IsRewardClaimed = isClaimed;

        public static RewardItem CreateDefault()
        {
            return new RewardItem
            {
                Id = 691,
                Amount = 10,
                IsRewardClaimed = false
            };
        }

        public void AddAmount(int delta) => 
            Amount += delta;
    }
}